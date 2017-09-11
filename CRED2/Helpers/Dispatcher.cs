using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CRED2.GitBridge
{
    public sealed class Dispatcher : IDisposable
    {
        public Dispatcher()
        {
            CancellationTokenSource = new CancellationTokenSource();
            ExclusivityBlock = new ActionBlock<Func<Task>>(func => func(), 
                new ExecutionDataflowBlockOptions { CancellationToken = CancellationTokenSource.Token });
        }

        private CancellationTokenSource CancellationTokenSource { get; }
        private ActionBlock<Func<Task>> ExclusivityBlock { get; }

        public void Post(Func<Task> value)
        {
            PostWithCompletion(value);
        }

        public Task<T> PostWithCompletion<T>(Func<Task<T>> value)
        {
            var tcs = new TaskCompletionSource<T>();
            ExclusivityBlock.Post(async () =>
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
            ExclusivityBlock.Post(async () =>
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

        public void Dispose()
        {
            ExclusivityBlock.Complete();
            CancellationTokenSource.Dispose();
        }
    }
}