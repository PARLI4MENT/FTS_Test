using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SQLNs;
using XmlFTS.OutClass;
using XMLSigner;

namespace XmlFTS
{
    public class ProcessHostXml
    {
        public static void StartServices()
        {

        }
    }

    public class FileSystemWatcherService : BackgroundService
    {
        private readonly string _directoryPath = @"C:\Test\RawFolder";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var watcher = new FileSystemWatcher(StaticPathConfiguration.PathRawFolder))
            {

                // Postavi željene filtere i događaje
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName;
                watcher.IncludeSubdirectories = false;
                watcher.Filter = "*.xml";
                watcher.InternalBufferSize = 4096;

                //watcher.Created += OnCreated;
                watcher.Changed += OnChanged;

                watcher.EnableRaisingEvents = true;

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
                watcher.EnableRaisingEvents = false;
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e) => Doing(); 

        private void Doing()
        {
            string MchdId = "e7d94ee1-33d4-4b95-a27d-07896fdc00e0";
            string MchdINN = "250908790897";
            X509Certificate2 cert = SignXmlGost.FindGostCurrentCertificate("01DA FCE9 BC8E 41B0 0008 7F5E 381D 0002");

            var rawSrcFiles = Directory.GetFiles(StaticPathConfiguration.PathRawFolder, "*.xml");
            if (rawSrcFiles.Count() != 0)
            {
                var countFiles = rawSrcFiles.Length;

                var sw = new Stopwatch();
                sw.Start();

                Parallel.ForEach
                    (rawSrcFiles,
                    new ParallelOptions { MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism },
                        xmlFile =>
                        {
                            if (Config.EnableBackup)
                                BackupFile.Backup(xmlFile, true);
                            try
                            {
                                using (StringReader stringReader = new StringReader(File.ReadAllText(xmlFile)))
                                {
                                    XmlDocument xmlDoc = new XmlDocument();
                                    xmlDoc.Load(stringReader);

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

                                    if (Config.DeleteSourceFiles)
                                        File.Delete(xmlFile);
                                }
                            }
                            catch (Exception ex) { Debug.WriteLine(ex.Message + " " + ex.Data); }
                            finally { }
                        });

                //if (true)
                //{
                //    Console.WriteLine();
                //    Console.WriteLine($"BaseProcess => {rawSrcFiles.Count()} count || {sw.Elapsed.TotalMilliseconds / (double)1000} sec.");
                //    //Console.WriteLine($"AVG (кол-во файлов / кол-во сек.) => {rawSrcFiles.Count() / (sw.ElapsedMilliseconds / 1000)}.");
                //}
                sw.Stop();

                if (Directory.GetFiles(StaticPathConfiguration.PathRawFolder).Length != 0)
                    Doing();

                Console.WriteLine($"Process main done!\nTime => {sw.Elapsed.TotalMilliseconds}\nFiles => {countFiles}");
            }
        }
    }

    /// <summary> </summary>
    public class BasicOperation : BackgroundService
    {
        public static int baseOperationDelay = 500;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(500);
            }
        }
        public async Task SortXml(string[] xmlFiles)
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
            return;
        }
    }

    public class ReplyProcess : BackgroundService
    {
        public static int replyOperationDelay = 500;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var rawSrcFolders = Directory.GetDirectories(StaticPathConfiguration.PathReplyFTS);

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