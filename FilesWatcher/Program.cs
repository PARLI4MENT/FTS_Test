using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FileWatching;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using XmlFTS.OutClass;

namespace FilesWatcher
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Config.BaseConfiguration("C:\\Test");
            Config.EnableBackup = false;
            Config.DeleteSourceFiles = true;
            Config.ReadAllSetting();

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<StartOperation, StartOperation>();
                    services.AddScoped<FileConsumerService, FileConsumerService>();
                })
                .Build();
            await host.RunAsync();
        }
    }
}