using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using XMLSigner;
using XMLSigner.OutClass;

namespace XmlFTS
{
    public class ProcessXML
    {
        public static bool IsStatistics { get; set; } = true;
        public static bool LogsIsEnable { get; set; } = false;

        /// <summary> Это нужно будет удалить </summary>
        static string MchdId = "e7d94ee1-33d4-4b95-a27d-07896fdc00e0";
        static string MchdINN = "250908790897";
        static X509Certificate2 cert = SignXmlGost.FindGostCurrentCertificate("01DA FCE9 BC8E 41B0 0008 7F5E 381D 0002");

        private ProcessXML() { }

        public static void StartProcess()
        {
            while (true)
            {
                var sw = new Stopwatch();
                sw.Start();
                int SummaryFiles = 0;
                Console.WriteLine("Sort start...");

                /// Получение путей папок из исходной папки
                var rawSrcFolders = Directory.GetDirectories("C:\\Dekl\\SEND DATA");
                foreach (var rawSrcFolder in rawSrcFolders)
                {
                    /// #1 Извлечение ZIP
                    ArchiveWorker.ExtractZipArchive(Directory.GetFiles(rawSrcFolder, "*.zip")[0]);

                    /// #2 Переименование и копирование
                    string[] xmlFiles = Directory.GetFiles(rawSrcFolder, "*.xml");
                    if (xmlFiles.Count() == 1)
                        RenamerXML.RenameMoveRawFiles(xmlFiles[0]);
                    if (xmlFiles.Count() > 1)
                        RenamerXML.RenameMoveRawFiles(xmlFiles);
                }

                ///// #3 Сортировка
                string[] notSortedFiles = Directory.GetFiles("C:\\_2\\ExtractionFiles", "*.xml");
                SortXml(notSortedFiles);

                SummaryFiles += Directory.GetFiles("C:\\_2\\ExtractionFiles", "*.xml").Count();

                sw.Stop();
                Console.WriteLine();
                Console.WriteLine($"General => {SummaryFiles} count || {sw.ElapsedMilliseconds / 1000} sec.");
                //Console.WriteLine($"AVG => {SummaryFiles / (sw.ElapsedMilliseconds / 1000)} sec.");

            }
            Debug.WriteLine("Process stoped");
            Debug.WriteLine("Press any key...");
            return;
        }

        public static void SortXml(string[] xmlFiles)
        {

            Parallel.ForEach
                (xmlFiles,
                new ParallelOptions { MaxDegreeOfParallelism = 1 },
                xmlFile =>
                {
                    if (File.Exists(xmlFile))
                    {
                        string tmpFilePath;

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(new StringReader(File.ReadAllText(xmlFile)));

                        switch (xmlDoc.DocumentElement.GetAttribute("DocumentModeID"))
                        {
                            /// ПТД ExpressCargoDeclaration
                            case "1006275E":
                                {
                                    tmpFilePath = Path.Combine("C:\\_2\\Sorted\\ptd", Path.GetFileName(xmlFile));
                                    //if (!File.Exists(tmpFilePath))
                                    //{
                                        File.Copy(xmlFile, tmpFilePath, true);

                                        // Шаблонизация + выбрать серификат Конкретного человека
                                        ImplementateToXml.ImplementLinear(tmpFilePath, ref cert, MchdId, MchdINN);
                                    //}
                                }
                                break;

                            /// В архив Остальное
                            default:
                                {
                                    tmpFilePath = Path.Combine("C:\\_2\\Sorted\\toArchive", Path.GetFileName(xmlFile));
                                    //if (!File.Exists(tmpFilePath))
                                    //{
                                        File.Copy(xmlFile, tmpFilePath, true);

                                        /// Шаблонизация + выбрать серификат (Компании) ///Пока индивидуальный
                                        ImplementateToXml.ImplementLinear(tmpFilePath, ref cert, MchdId, MchdINN);
                                    //}
                                }
                                break;
                        }
                    }
                });
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                Debug.WriteLine(e.FullPath);
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                Console.WriteLine($"Created: {e.FullPath}");
            }
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                Console.WriteLine($"Deleted: {e.FullPath}");
            }
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Renamed)
            {
                Console.WriteLine($"Renamed:");
                Console.WriteLine($"    Old: {e.OldFullPath}");
                Console.WriteLine($"    New: {e.FullPath}");
            }
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            PrintException(e.GetException());
        }

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
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
            string path = @"C:\YourFolder";

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