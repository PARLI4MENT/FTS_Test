using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SQLNs;
using XmlFTS.OutClass;
using XMLSigner;

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
                       services.AddHostedService<BasicOperation>();
                       //services.AddHostedService<ReplyProcessTick>();
                       //services.AddHostedService<CheckBackup>();
                   })
                   .UseConsoleLifetime().Build();

            host.Run();
        }
    }

    /// <summary> </summary>
    internal class BasicOperation : BackgroundService
    {
        public static int baseOperationDelay = 500;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var rawSrcFiles = Directory.GetFiles("C:\\Test\\RawFolder", "*.xml");

                if (rawSrcFiles.Count() != 0)
                {
                    var sw = new Stopwatch();
                    if (true)
                        sw.Start();

                    Console.WriteLine("Start basic process...");

                    ///// #3 Сортировка
                    SortXml(rawSrcFiles);

                    if (true)
                    {
                        sw.Stop();
                        Console.WriteLine();
                        Console.WriteLine($"BaseProcess => {rawSrcFiles.Count()} count || {sw.Elapsed.TotalMilliseconds / (double)1000} sec.");
                        Console.WriteLine($"AVG (кол-во файлов / кол-во сек.) => {rawSrcFiles.Count() / (sw.ElapsedMilliseconds / 1000)}.");
                    }
                    Console.WriteLine("Process main done!");
                }
                await Task.Delay(500);
            }
        }
        public void SortXml(string[] xmlFiles)
        {
            string MchdId = "e7d94ee1-33d4-4b95-a27d-07896fdc00e0";
            string MchdINN = "250908790897";
            X509Certificate2 cert = SignXmlGost.FindGostCurrentCertificate("01DA FCE9 BC8E 41B0 0008 7F5E 381D 0002");

            Parallel.ForEach
                        (xmlFiles,
                        new ParallelOptions { MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism },
                        xmlFile =>
                        {
                            if (Config.EnableBackup)
                                BackupFile.Backup(xmlFile, true);

                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(new StringReader(File.ReadAllText(xmlFile)));

                            switch (xmlDoc.DocumentElement.GetAttribute("DocumentModeID"))
                            {
                                /// ПТД ExpressCargoDeclaration
                                case "1006275E":
                                    // Шаблонизация + выбрать серификат Конкретного человека
                                    TemplatingXml.TemplatingLinear(xmlFile, ref cert, MchdId, MchdINN);
                                    break;

                                /// В архив Остальное
                                default:
                                    // Шаблонизация + выбрать серификат (Компании) ///Пока индивидуальный
                                    TemplatingXml.TemplatingLinear(xmlFile, ref cert, MchdId, MchdINN);
                                    break;
                            }
                        });
        }
    }

    internal class ReplyProcessTick : BackgroundService
    {
        public static int replyOperationDelay = 500;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var rawSrcFolders = Directory.GetDirectories("C:\\_2\\ReplyFTS");

                if (rawSrcFolders.Count() == 0)
                {
                    await Task.Delay(replyOperationDelay);
                    break;
                }
            }
        }

        private static void ReplyXml(string[] xmlFiles)
        {
            Parallel.ForEach(xmlFiles,
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                xmlFile =>
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(new StringReader(File.ReadAllText(xmlFile)));
                    string InitialEnvelopeID = xmlDoc.DocumentElement.GetElementsByTagName("roi:InitialEnvelopeID")[0].InnerText.ToUpper();
                    //string DocumentID = xmlDoc.DocumentElement.GetElementsByTagName("ct:DocumentID")[0].InnerText.ToUpper();
                    string ResultDescription = xmlDoc.DocumentElement.GetElementsByTagName("rslt:ResultDescription")[0].InnerText;
                    ///
                    new PgSql().PgRetriveData(InitialEnvelopeID, InitialEnvelopeID, ResultDescription);

                    if (Config.EnableBackup)
                        File.Copy(xmlFile, Path.Combine("C:\\Test\\BackupReplyFTS", Path.GetFileName(xmlFile)), true);
                    File.Delete(xmlFile);
                });
        }
    }

    internal class CheckBackup : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}