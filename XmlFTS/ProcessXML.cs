using SQLNs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using XmlFTS.OutClass;
using XMLSigner;
using Timer = System.Timers.Timer;

namespace XmlFTS
{
    public class ProcessXML
    {
        public static bool IsStatistics { get; set; } = true;
        public static bool LogsIsEnable { get; set; } = false;

        /// <summary> Это нужно будет удалить </summary>
        private static string MchdId = "e7d94ee1-33d4-4b95-a27d-07896fdc00e0";
        private static string MchdINN = "250908790897";
        private static X509Certificate2 cert = SignXmlGost.FindGostCurrentCertificate("01DA FCE9 BC8E 41B0 0008 7F5E 381D 0002");

        private static Timer BaseProcessTimer;
        private static Timer ReplyProcessTimer;
        private static DateTime lastWrite;

        public static void ProcessStart()
        {
            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            /// Начальная обработка Xml-файлов
            var BaseProcess = Task.Run(() =>
            {
                Console.WriteLine($"Base Process => Starting");
                BaseProcessTimer = new Timer();
                BaseProcessTimer.Interval = 500;
                BaseProcessTimer.Elapsed += new ElapsedEventHandler(BaseTick);
                BaseProcessTimer.Start();
            });

            /// Обработка
            //var ReplyProcess = Task.Run(() =>
            //{
            //    Console.WriteLine("Reply FTS Process => Starting");
            //    ReplyProcessTimer = new System.Timers.Timer();
            //    ReplyProcessTimer.Interval = 100;
            //    ReplyProcessTimer.Elapsed += new ElapsedEventHandler(ReplyProcessTick);
            //    ReplyProcessTimer.Start();
            //});
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e) { }

        /// <summary>Задача => Начальная обработка </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void BaseTick(object sender, ElapsedEventArgs e)
        {
            FileInfo info = new FileInfo(StaticPathConfiguration.PathRawFolder);

            if (lastWrite == DateTime.MinValue)
                lastWrite = info.LastWriteTime;

            if (lastWrite.CompareTo(info.LastWriteTime) == -1)
            {
                BaseProcessTimer.Stop();

                var rawSrcFolders = Directory.GetDirectories(StaticPathConfiguration.PathRawFolder);

                if (rawSrcFolders != null)
                    foreach (var rawSrcFolder in rawSrcFolders)
                    {
                        Debug.WriteLine("Start main process...");

                        int SummaryFiles = 0;

                        var sw = new Stopwatch();
                        if (IsStatistics)
                            sw.Start();

                        /// #1 Извлечение ZIP
                        ArchiveWorker.ExtractZipArchive(Directory.GetFiles(rawSrcFolder, "*.zip")[0]);
                        if (IsStatistics)
                            SummaryFiles += Directory.GetFiles(StaticPathConfiguration.PathExtractionFolder, "*.xml").Count();

                        /// #2 Переименование и копирование
                        string[] xmlFiles = Directory.GetFiles(rawSrcFolder, "*.xml");
                        if (xmlFiles.Count() == 1)
                            RenamerXML.RenameMoveRawFiles(xmlFiles[0]);
                        if (xmlFiles.Count() > 1)
                            RenamerXML.RenameMoveRawFiles(xmlFiles);

                        /// Remove srcFolder
                        Directory.Delete(rawSrcFolder, true);

                        ///// #3 Сортировка
                        SortXml(Directory.GetFiles(StaticPathConfiguration.PathExtractionFolder, "*.xml"));

                        if (IsStatistics)
                        {
                            sw.Stop();
                            Console.WriteLine();
                            Console.WriteLine($"BaseProcess => {SummaryFiles} count || {sw.ElapsedMilliseconds / 1000} sec.");
                            //Console.WriteLine($"AVG (кол-во файлов / кол-во сек.) => {SummaryFiles / (sw.ElapsedMilliseconds / 1000)}.");
                        }
                        Debug.WriteLine("Process main done!");
                    }

                BaseProcessTimer.Start();
            }
        }

        /// <summary>Задача => Последующая обработка </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void ReplyProcessTick(object sender, ElapsedEventArgs e)
        {
            FileInfo info = new FileInfo(StaticPathConfiguration.PathReplyFTS);

            if (lastWrite == DateTime.MinValue)
                lastWrite = info.LastWriteTime;

            if (lastWrite.CompareTo(info.LastWriteTime) == -1)
            {
                var replyFiles = Directory.GetFiles(StaticPathConfiguration.PathReplyFTS);

                if (replyFiles.Count() != 0)
                {
                    ReplyProcessTimer.Stop();

                    int SummaryFiles = 0;

                    var sw = new Stopwatch();
                    if (IsStatistics)
                        sw.Start();

                    foreach (string replyFile in replyFiles)
                    {
                        SummaryFiles += replyFiles.Count();

                        ReplyXml(replyFiles);
                    }

                    if (IsStatistics)
                    {
                        sw.Stop();
                        Console.WriteLine();
                        Console.WriteLine($"Reply FTS Process => {SummaryFiles} count || {sw.ElapsedMilliseconds / 1000} sec.");
                        Console.WriteLine($"AVG (кол-во файлов / кол-во сек.) => {SummaryFiles / (sw.ElapsedMilliseconds / 1000)}.");
                    }
                    Debug.WriteLine("Process done!");

                    ReplyProcessTimer.Stop();
                }
            }
        }

        private static void SortXml(string[] xmlFiles)
        {
            if (xmlFiles == null)
                return;

            Parallel.ForEach
                (xmlFiles,
                new ParallelOptions { MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism },
                xmlFile =>
                {
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

        private static void ReplyXml(string[] xmlFiles)
        {
            Parallel.ForEach(xmlFiles,
                new ParallelOptions { MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism },
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
                        File.Copy(xmlFile, Path.Combine("C:\\_2\\BackupReplyFTS", Path.GetFileName(xmlFile)));
                    File.Delete(xmlFile);
                });
        }
    }
}
namespace FolderWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            // Путь к отслеживаемой папке
            string path = @"C:\_1";

            // Создаем объект FileSystemWatcher
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;

            // Настраиваем типы событий, на которые будем реагировать
            watcher.NotifyFilter = NotifyFilters.LastWrite
                                  | NotifyFilters.FileName
                                  | NotifyFilters.DirectoryName;

            // Обработчики событий
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            // Запускаем отслеживание
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Изменение файла: {e.FullPath}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Создан файл: {e.FullPath}");
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Удален файл: {e.FullPath}");
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Переименован файл: {e.OldFullPath} -> {e.FullPath}");
        }
    }
}