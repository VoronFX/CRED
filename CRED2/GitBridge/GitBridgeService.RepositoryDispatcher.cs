using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CRED2.GitRepository
{


	public sealed partial class GitBridgeService
	{
		private sealed class RepositoryDispatcher : IDisposable
		{
			public RepositoryDispatcher()
			{
				Task.Run(DispatcherLoop);
			}

			private ConcurrentQueue<Func<Task>> RepoTasks { get; } = new ConcurrentQueue<Func<Task>>();

			private volatile TaskCompletionSource<bool> dispatcherSleep;

			private CancellationTokenSource DispatcherStop { get; }
				= new CancellationTokenSource();

			public Task InvokeAsync(Func<Task> action)
			{
				//TODO: CancellationSupport
				TaskCompletionSource<bool> awaiter
					= new TaskCompletionSource<bool>();
				RepoTasks.Enqueue(async () =>
				{
					await action();
					awaiter.SetResult(true);
				});
				dispatcherSleep?.TrySetResult(true);
				return awaiter.Task;
			}

			public Task InvokeAsync(Action action)
			{
				return InvokeAsync(() => Task.Run(action));
			}

			private async Task DispatcherLoop()
			{
				while (!DispatcherStop.IsCancellationRequested)
				{
					if (!RepoTasks.TryDequeue(out var nextTask))
					{
						dispatcherSleep = new TaskCompletionSource<bool>();
						if (!RepoTasks.TryDequeue(out nextTask))
						{
							await dispatcherSleep.Task;
							dispatcherSleep = null;
							continue;
						}
						else
						{
							dispatcherSleep = null;
						}
					}
					await nextTask();
				}
			}

			public void Dispose()
			{
				DispatcherStop.Cancel();
			}
		}
	}
}