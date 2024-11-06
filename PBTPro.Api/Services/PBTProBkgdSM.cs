using System.Collections.Concurrent;

namespace PBTPro.Api.Services
{
    public class PBTProBkgdSM : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ConcurrentDictionary<string, PBTProBackgroundService> backgroundServices;

        public PBTProBkgdSM(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.backgroundServices = new ConcurrentDictionary<string, PBTProBackgroundService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Initialization logic if needed
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop all running background services
            foreach (var service in backgroundServices.Values)
            {
                service.StopAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        public void StartBackgroundService(string serviceName)
        {
            var queue = new PBTProBkgdWorkerQueue();
            var backgroundService = new PBTProBackgroundService(queue);

            backgroundServices.TryAdd(serviceName, backgroundService);
            backgroundService.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        public void StopBackgroundService(string serviceName)
        {
            if (backgroundServices.TryGetValue(serviceName, out var backgroundService))
            {
                backgroundService.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            }
            else
            {
                throw new InvalidOperationException($"Service '{serviceName}' does not exist.");
            }
        }

        public void RemoveBackgroundService(string serviceName)
        {
            if (backgroundServices.TryRemove(serviceName, out var backgroundService))
            {
                backgroundService.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            }
            else
            {
                throw new InvalidOperationException($"Service '{serviceName}' does not exist.");
            }
        }

        public void EnqueueWorkItem(string serviceName, Func<CancellationToken, Task> workItem)
        {
            if (backgroundServices.TryGetValue(serviceName, out var backgroundService))
            {
                backgroundService.Queue.QueueWorkItem(workItem);
            }
            else
            {
                throw new InvalidOperationException($"Service '{serviceName}' does not exist.");
            }
        }

        public IDictionary<string, bool> GetAllBackgroundServiceStatus()
        {
            return backgroundServices.ToDictionary(kv => kv.Key, kv => kv.Value.IsRunning);
        }

        public PBTProBkgdWorkerQueue GetBackgroundServiceQueue(string serviceName)
        {
            if (backgroundServices.TryGetValue(serviceName, out var backgroundService))
            {
                return backgroundService.Queue;
            }

            return null;
        }
    }
}
