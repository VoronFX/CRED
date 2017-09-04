using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CRED2.GitRepository
{


	public sealed partial class Service
	{
		private sealed class RepositoryDispatcher : IDisposable
		{
			public RepositoryDispatcher()
			{
				Task.Run(DispatcherLoop);
			}

			private ConcurrentQueue<Task> RepoTasks { get; } = new ConcurrentQueue<Task>();

			private volatile TaskCompletionSource<bool> dispatcherSleep;

			private CancellationTokenSource DispatcherStop { get; }
				= new CancellationTokenSource();

			public Task InvokeAsync(Action action)
			{
				var task = new Task(action);
				RepoTasks.Enqueue(task);
				dispatcherSleep?.TrySetResult(true);
				return task;
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
					await Task.Run(() => nextTask);
				}
			}

			public void Dispose()
			{
				DispatcherStop.Cancel();
			}
		}
	}
}