using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CRED2.GitBridge;
using CRED2.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CRED2.Services
{
    public sealed class TaskRequestService : IDisposable
    {
        private string TaskRequestCacheKey(long id) => id + nameof(TaskRequest);
        private Dispatcher Dispatcher { get; } = new Dispatcher();
        private HistoryRepository Database { get; }
        private IMemoryCache MemoryCache { get; }
        private IServiceProvider ServiceProvider { get; }
        //private ImmutableDictionary<string, Type> TaskRequestRunners { get; }

        public TaskRequestService(HistoryRepository database, IMemoryCache memoryCache,
            IServiceProvider serviceProvider)
        {
            Database = database;
            MemoryCache = memoryCache;
            ServiceProvider = serviceProvider;
            //TaskRequestRunners = Assembly.GetExecutingAssembly().DefinedTypes
            //    .Where(x => x.ImplementedInterfaces
            //        .Any(x2 => x2.IsConstructedGenericType &&
            //                   x2.GetGenericTypeDefinition() == typeof(ITaskRequestRunner<>)))
            //    .ToImmutableDictionary(x => x.ImplementedInterfaces
            //        .First(x2 =>
            //            x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(ITaskRequestRunner<>))
            //        .GetGenericArguments()
            //        .First()
            //        .FullName, info => info.AsType());
            foreach (var taskRequest in Database
                .Fetch<TaskRequest>(x => x.State == TaskRequestState.RequestRecieved))
            {
                var type = Type.GetType(taskRequest.RequestDataType);
                var requestData = Convert.ChangeType(taskRequest.RequestData, type);
                var runnerType = typeof(ITaskRequestRunner<>).MakeGenericType(type);
                var runner = ServiceProvider.GetRequiredService(runnerType);
                runnerType.GetMethod(nameof(ITaskRequestRunner<object>.Run)).Invoke(runner, new[] { requestData });
            }
        }


        public Task<long> CreateTaskRequest(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var transactionRequest = new TaskRequest { Created = DateTime.UtcNow };
                if (!cancellationToken.IsCancellationRequested)
                    Database.Insert(transactionRequest);
                Dispatcher.PostWithCompletion(() =>
                    MemoryCache.GetOrCreateAsync(TaskRequestCacheKey(transactionRequest.Id),
                        entry => Task.FromResult(transactionRequest)));
                return transactionRequest.Id;
            }, cancellationToken);
        }

        public Task<TaskRequest> GetTaskRequest(long id, CancellationToken cancellationToken)
        {
            return MemoryCache.GetOrCreateAsync(id + nameof(TaskRequest), entry =>
                Task.Run(() => Database.SingleById<TaskRequest>(id), cancellationToken));
        }

        public Task OpenTaskRequest(long taskRequestId, object requestData, CancellationToken cancellationToken)
        {
            var type = requestData.GetType();
            return (Task)GetType().GetMethods().First(m => m.Name == nameof(OpenTaskRequest) && m.IsGenericMethod)
                .MakeGenericMethod(type).Invoke(this, new[] { taskRequestId, requestData, cancellationToken });
        }

        public async Task OpenTaskRequest<T>(long taskRequestId, T requestData, CancellationToken cancellationToken)
        {
            var taskRequest = await GetTaskRequest(taskRequestId, cancellationToken);

            if (taskRequest.State == TaskRequestState.Created)
            {
                ITaskRequestRunner<T> runner = null;
                if (await Dispatcher.PostWithCompletion(async () =>
                {
                    taskRequest = await GetTaskRequest(taskRequestId, cancellationToken);
                    if (taskRequest.State == TaskRequestState.Created)
                    {
                        taskRequest.State = TaskRequestState.RequestRecieved;
                        taskRequest.RequestDataType = typeof(T).FullName;
                        taskRequest.RequestData = Database.Database.Mapper.ToDocument(requestData);
                        runner = ServiceProvider.GetRequiredService<ITaskRequestRunner<T>>();
                        Database.Update(taskRequest);
                        MemoryCache.Set(taskRequestId + nameof(TaskRequest), taskRequest);
                        return true;
                    }
                    return false;
                }))
                {
                    runner.Run(taskRequest, requestData);
                    return;
                }
            }
            if (!taskRequest.RequestData.Equals(Database.Database.Mapper.ToDocument(requestData)))
                throw new Exception("Attempt to open TaskRequest already opened with different input data.");
        }

        public static IEnumerable<Type> DiscoverRunnerTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(x => DiscoverImplementedRunnerInterfaces(x).Any());
        }

        public static IEnumerable<Type> DiscoverImplementedRunnerInterfaces(Type type)
        {
            return type.GetInterfaces()
                .Where(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(ITaskRequestRunner<>));
        }

        public static Type RunnerInterfaceRequestDataType(Type type)
        {
            return type.GetGenericArguments().First();
        }

        public void Dispose()
        {
            Dispatcher.Dispose();
        }
    }

    public interface ITaskRequestRunner<in T>
    {
        void Run(TaskRequest taskRequest, T requestData);
    }
}