using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using XmlFTS.OutClass;
using XmlFTS;

namespace FileWatching
{
    public class FileConsumerService
    {
        ILogger<FileConsumerService> _logger;

        public FileConsumerService(ILogger<FileConsumerService> logger)
        {
            _logger = logger;
        }

        public async Task ConsumeFile(string pathToFile)
        {
            var rawSrcFolders = Directory.GetDirectories("C:\\Test\\RawFolder");

            int SummaryFiles = 0;
            var sw = new Stopwatch();
            if (IsStatistics)
                sw.Start();

            if (rawSrcFolders != null)
            {
                foreach (var rawSrcFolder in rawSrcFolders)
                {
                    Debug.WriteLine("Start main process...");

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
                }
                if (IsStatistics)
                {
                    sw.Stop();
                    Console.WriteLine();
                    Console.WriteLine($"BaseProcess => {SummaryFiles} count || {sw.ElapsedMilliseconds / 1000} sec.");
                    Console.WriteLine($"AVG (кол-во файлов / кол-во сек.) => {SummaryFiles / (sw.ElapsedMilliseconds / 1000)}.");
                }
                Debug.WriteLine("Process main done!");
            }
    }
}