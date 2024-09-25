using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace XmlFTS
{
    /// <summary> TEST ONLY </summary>
    public static class ProcessHostXML
    {
        public static async Task RunProcess()
        {
            var host = new HostBuilder()
                   .ConfigureHostConfiguration(hConfig => { })
                   .ConfigureServices((context, services) =>
                   {
                       services.AddSingleton<MyProcess1>();
                       services.AddHostedService<MyProcess2>();
                   })
                   .UseConsoleLifetime().Build();

            await host.RunAsync();
        }
    }

    public class MyProcess1 : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"Process1 Singleton => {DateTime.Now.Date}");
                await Task.Delay(1000);
            }
        }
    }

    public class MyProcess2 : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"Process 2 HostedService => {DateTime.Now.Date}");
                //await Task.Delay(5000);
            }
        }
    }
}