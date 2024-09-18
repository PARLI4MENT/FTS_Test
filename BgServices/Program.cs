using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BgServices
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host1 = new HostBuilder()
                .ConfigureHostConfiguration(hconfig => { })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<WatchBaseFolder>();
                    services.AddHostedService<MyProcessor2>();
                })
                .UseConsoleLifetime().Build();

            await host1.RunAsync();
        }
    }

    /// <summary> Отслеживание начальной папки</summary>
    public class WatchBaseFolder : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                /// Сюда
                
                await Task.Delay(500);
            }
            return;
        }
    }

    public class MyProcessor2 : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                /// Сюда

                await Task.Delay(500);
            }
            return;
        }
    }
}