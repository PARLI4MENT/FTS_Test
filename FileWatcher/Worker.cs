using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileWatching;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly MyFileWatcher _watcher;

    public Worker(ILogger<Worker> logger, MyFileWatcher watcher)
    {
        _logger = logger;
        _watcher = watcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _watcher.Start();
        while (!stoppingToken.IsCancellationRequested)
        {

        }
    }
}