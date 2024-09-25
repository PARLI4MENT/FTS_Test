using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystemWatcherMy
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .RunConsoleAsync();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder()
        //    .ConfigureServices((HostBuilderContext, services) =>
        //    {
        //        services.AddHostedService<Worker>();
        //    });
    }

    //public class Worker: BackgroundService
    //{
    //    private readonly ILogger<Worker> _logger;

    //    public Worker(ILogger<Worker> logger)
    //    {
    //        _logger = logger;
    //    }

    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        while (!stoppingToken.IsCancellationRequested)
    //        {
    //            _logger.LogInformation("Worker ... ", DateTimeOffset.Now);
    //            await Task.Delay(1000, stoppingToken);

    //            var watcher = new FileSystemWatcher("");
    //            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime |
    //                NotifyFilters.FileName | NotifyFilters.Size;
    //            watcher.Filter = "*.xml";

    //            watcher.Changed += new FileSystemEventHandler(onChange);
    //            watcher.Created += new FileSystemEventHandler(onChange);

    //            /// Start monitoring
    //            watcher.EnableRaisingEvents = true;
    //        }
    //    }

    //    public void onChange(object sender, FileSystemEventArgs e)
    //    {
    //        Console.WriteLine(e.Name + " ", e.ChangeType);
    //    }
    //}
}