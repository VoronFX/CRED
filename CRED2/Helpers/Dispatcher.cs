using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CRED2.Helpers
{
    public sealed class Dispatcher : IDisposable
    {
        public Dispatcher()
        {
            this.CancellationTokenSource = new CancellationTokenSource();
            this.ExclusivityBlock = new ActionBlock<Func<Task>>(
                func => func(),
                new ExecutionDataflowBlockOptions { CancellationToken = this.CancellationTokenSource.Token });
        }

        private CancellationTokenSource CancellationTokenSource { get; }

        private ActionBlock<Func<Task>> ExclusivityBlock { get; }

        public void Dispose()
        {
            this.ExclusivityBlock.Complete();
            this.CancellationTokenSource.Dispose();
        }

        public void Post(Func<Task> value)
        {
            this.PostWithCompletion(value);
        }

        public Task<T> PostWithCompletion<T>(Func<Task<T>> value)
        {
            var tcs = new TaskCompletionSource<T>();
            this.ExclusivityBlock.Post(
                async () =>
                    {
                        try
                        {
                            tcs.TrySetResult(await value());
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    });
            return tcs.Task;
        }

        public Task PostWithCompletion(Func<Task> value)
        {
            var tcs = new TaskCompletionSource<object>();
            this.ExclusivityBlock.Post(
                async () =>
                    {
                        try
                        {
                            await value();
                            tcs.TrySetResult(null);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    });
            return tcs.Task;
        }
    }
}