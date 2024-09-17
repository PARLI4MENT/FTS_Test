using FileWatching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<MyFileWatcher, MyFileWatcher>();
        services.AddScoped<FileConsumerService, FileConsumerService>();

    })
    .Build();

await host.RunAsync();