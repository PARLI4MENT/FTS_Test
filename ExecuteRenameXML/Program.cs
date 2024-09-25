using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ExecuteRenameXML
{
    internal class Program
    {
        static string inputPath = "C:\\Dekl\\SEND DATA";
        static string PathExtractionFolder = "C:\\_2\\ExtractionFolder";

        static void Main(string[] args) {
            var rawSrcFolders = Directory.GetDirectories(inputPath);

            var sw = Stopwatch.StartNew();
            if (rawSrcFolders != null)
            {
                Parallel.ForEach(rawSrcFolders,
                    rawSrcFolder => {
                        Console.WriteLine($@"Started => {rawSrcFolder}");

                        /// #1 Извлечение ZIP
                        ExtractZipArchive(Directory.GetFiles(rawSrcFolder, "*.zip"));

                        /// #2 Переименование и копирование
                        string[] xmlFiles = Directory.GetFiles(rawSrcFolder, "*.xml");
                        RenameMoveRawFiles(xmlFiles);
                    });

                sw.Stop();
                Console.WriteLine($@"Time is: {(int)sw.Elapsed.TotalSeconds}");
                Console.WriteLine("Process main done!");
                Console.ReadKey();
            }
        }

        public static void ExtractZipArchive(string[] pathToZips)
        {
            foreach (string pathToZip in pathToZips) {
                string code = Path.GetFileName(Path.GetDirectoryName(pathToZip));

                using (ZipArchive zipArch = new ZipArchive(File.OpenRead(pathToZip)))
                {
                    foreach (var entry in zipArch.Entries)
                    {
                        if (entry.FullName.ToLower().Contains("xml"))
                        {
                            string pathDest = Path.Combine(PathExtractionFolder, string.Concat(code, ".", entry.Name));
                            entry.ExtractToFile(pathDest, true);
                        }
                    }
                }
            }
        }

        public static void RenameMoveRawFiles(string[] xmlFiles)
        {
            foreach (var xmlFile in xmlFiles)
            {
                string code = Path.GetFileName(Path.GetDirectoryName(xmlFile));
                string tmpPathCombine = Path.Combine(PathExtractionFolder, string.Concat(code, ".", Path.GetFileName(xmlFile)));

                if (!File.Exists(tmpPathCombine))
                    File.Copy(xmlFile, tmpPathCombine, true);
            }
        }

    }
}
