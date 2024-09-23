using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;

namespace FileWatching
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly StartOperation _watcher;

        public Worker(ILogger<Worker> logger, StartOperation watcher)
        {
            _logger = logger;
            _watcher = watcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _watcher.Start();
            while (!stoppingToken.IsCancellationRequested) { }
        }
    }
}