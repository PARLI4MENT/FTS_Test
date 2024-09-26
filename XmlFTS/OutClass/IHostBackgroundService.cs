using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XmlFTS.OutClass
{
    internal class IHostBackgroundService
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<MyServiceOptions>(hostContext.Configuration);
                    services.AddHostedService<MyService>();
                    services.AddSingleton(Console.Out);
                });
        }
    }
    public class MyService : IHostedService
    {
        private readonly MyServiceOptions _options;
        private readonly TextWriter _outputWriter;

        public MyService(TextWriter outputWriter, IOptions<MyServiceOptions> options)
        {
            _options = options.Value;
            _outputWriter = outputWriter;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _outputWriter.WriteLine("Starting work");

            DoOperation(_options.OpCode, _options.Operand);

            _outputWriter.WriteLine("Work complete");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _outputWriter.WriteLine("StopAsync");
        }

        protected void DoOperation(int opCode, int operand)
        {
            _outputWriter.WriteLine("Doing {0} to {1}...", opCode, operand);


        }
    }

    public class MyServiceOptions
    {
        public int OpCode { get; set; }
        public int Operand { get; set; }
    }
}
