using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FileWatching;
using System.Threading.Tasks;

namespace FilesWatcher
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<MyFileWatcher, MyFileWatcher>();
                    services.AddScoped<FileConsumerService, FileConsumerService>();
                })
                .Build();
            await host.RunAsync();
        }
    }
}