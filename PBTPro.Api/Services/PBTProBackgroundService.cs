namespace PBTPro.Api.Services
{
    public class PBTProBackgroundService : BackgroundService
    {
        private readonly PBTProBkgdWorkerQueue queue;
        private bool isRunning;

        public PBTProBackgroundService(PBTProBkgdWorkerQueue queue)
        {
            this.queue = queue;
            this.isRunning = false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            isRunning = true;

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    stoppingToken.ThrowIfCancellationRequested();

                    var workItem = await queue.DequeueAsync(stoppingToken);
                    await workItem(stoppingToken);
                }
            }
            finally
            {
                isRunning = false;
            }
        }
        public bool IsRunning => isRunning;
        public PBTProBkgdWorkerQueue Queue => queue;
    }
}
