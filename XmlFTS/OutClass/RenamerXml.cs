#define DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using XmlFTS.OutClass;

namespace FileNs
{
    public class RenamerXML
    {
        static string _rootDir = "C:\\_test";
        public static string RootDir { get { return _rootDir; } set { _rootDir = value; } }

        private static string _dirInput = Path.Combine(_rootDir, "rawFiles");

        private static string _dirDestination = Path.Combine(_rootDir, "inputFiles");


        private static long _elapsedMilliseconds { get; set; }
        private static int _countRawFiles { get; set; }
        private static int _subFolder { get; set; }

        

        /// <summary> Линейное переименование и перемещение сырых Xml-файлов </summary>
        /// <param name="rawFolders"> </param>
        /// <param name="_MaxDegreeOfParallelism"></param>
        public static void RenameMove(string[] rawFolders)
        {
            foreach (var rawFolder in rawFolders)
            {
                string[] subDir = Directory.GetDirectories(Path.Combine(rawFolder, "files"));
                string[] filesSubfolder = (Directory.GetFiles(Path.Combine(subDir[0], "xml")));

                foreach (string file in filesSubfolder)
                {
                    // Combine Xml
                    string tmpPathCombine = Path.Combine(StaticPathConfiguration.PathIntermidateFolder, string.Concat(Path.GetFileName(rawFolder), ".", Path.GetFileName(file)));
                    File.Copy(file, tmpPathCombine);
                }
            }
        }

        /// <summary> Распраллеленое переименование и перемещение "сырых" Xml-файлов </summary>
        /// <remarks>
        /// Максимальная степень распалаллеленности определяется в файле конфигурации (По-умолчанию = 4)
        /// </remarks>
        /// <param name="rawFolders"> Set array with raw files </param>
        public static void RenameMoveParallel(string[] rawFolders)
        {
            Parallel.ForEach(rawFolders,
                new ParallelOptions { MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism },
                rawFolder =>
                {
                    string[] subDir = Directory.GetDirectories(Path.Combine(rawFolder, "files"));
                    string[] filesSubfolder = (Directory.GetFiles(Path.Combine(subDir[0], "xml")));
                    foreach (string file in filesSubfolder)
                    {
                        // Combine Xml
                        string tmpPathCombine = Path.Combine("C:\\_test\\intermidateFiles", string.Concat(Path.GetFileName(rawFolder), ".", Path.GetFileName(file)));
                        File.Copy(file, tmpPathCombine);
                    }
                });
        }

        /// <summary> Линейное переименование и перемещение сырых Xml-файлов </summary>
        /// <param name="pathRawFiles">Путь к папке с сырыми Xml-файлами</param>
        public static void RenameMove(string pathRawFiles)
        {
            var rawFolder = Directory.GetDirectories(pathRawFiles);
            foreach (var rawFile in rawFolder)
            {
                string[] subDir = Directory.GetDirectories(Path.Combine(rawFile, "files"));
                string[] filesSubfolder = (Directory.GetFiles(Path.Combine(subDir[0], "xml")));

                foreach (string file in filesSubfolder)
                {
                    // Combine Xml
                    string tmpPathCombine = Path.Combine("C:\\_test\\intermidateFiles", string.Concat(Path.GetFileName(rawFile), ".", Path.GetFileName(file)));
                    File.Copy(file, tmpPathCombine);
                }
            }
        }

        /// <summary> Параллельное переименование и перемещение сырых Xml-файлов </summary>
        /// <param name="pathRawFiles"> Set folder with raw files </param>
        public static void RenameMoveParallel(string pathRawFiles)
        {
            var rawFolder = Directory.GetDirectories(pathRawFiles);
            Parallel.ForEach(rawFolder,
                new ParallelOptions { MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism },
                rawFile =>
                {
                    string[] subDir = Directory.GetDirectories(Path.Combine(rawFile, "files"));
                    string[] filesSubfolder = (Directory.GetFiles(Path.Combine(subDir[0], "xml")));

                    foreach (string file in filesSubfolder)
                    {
                        // Combine Xml
                        string tmpPathCombine = Path.Combine("C:\\_test\\intermidateFiles", string.Concat(Path.GetFileName(rawFile), ".", Path.GetFileName(file)));
                        File.Copy(file, tmpPathCombine, true);
                    }
                });
        }

        /// <summary> Перемещение и переименование Xml-файлов по маске </summary>
        /// <param name="rawFile">Путь к папке с исходными файлами</param>
        /// <param name="destinationPath">Путь к папке назначения</param>
        /// <param name="deletedRawFolder">Удалять исходную папку</param>
        public void RenameAndMove([Optional] string rawFile, [Optional] string destinationPath, bool deletedRawFolder = false)
        {
            {
                var inputFiles = Directory.GetFiles(_dirDestination);
                if (inputFiles.Count() != 0)
                    foreach (var inFile in inputFiles)
                        File.Delete(inFile);
            }

            Console.WriteLine("Start rename && move...");

            if (!string.IsNullOrEmpty(rawFile))
                _dirInput = rawFile;

            if (!string.IsNullOrEmpty(destinationPath))
                _dirDestination = destinationPath;

            // Timer
            var swRename = new Stopwatch();
            swRename.Start();

            var baseFolder = new List<string>();
            baseFolder.AddRange(Directory.GetDirectories(_dirInput).ToList<string>());

            int subFolder = 0;
            foreach (var dir in baseFolder)
            {
                /// Тут можно упростить =>
                string tmpSubfolder = Path.GetFileName(dir);
                string[] subDir = Directory.GetDirectories(Path.Combine(dir, "files"));

                List<string> filesSubfolder = new List<string>();
                filesSubfolder.AddRange(Directory.GetFiles(Path.Combine(subDir[0], "xml")));

                foreach (string file in filesSubfolder)
                {
                    subFolder += filesSubfolder.Count;
                    Task.Run(() =>
                        File.Copy(file, Path.Combine(_dirDestination, string.Concat(tmpSubfolder, ".", Path.GetFileName(file)))));
                }
            }

#if DEBUG
            swRename.Stop();
            _elapsedMilliseconds = swRename.ElapsedMilliseconds;
            _countRawFiles = baseFolder.Count();
#endif
            Console.WriteLine("{\n");

            // Delete non usable base folder
            if (deletedRawFolder)
                Task.Run(() => Delete(""));
        }

        /// <summary> Паралельное перемещение и переименование Xml-файлов по маске </summary>
        /// <param name="rawFile">Путь к папке с исходными файлами</param>
        /// <param name="destinationPath">Путь к папке назначения</param>
        /// <param name="deletedRawFolder">Удалять исходную папку</param>
        public void RenameAndMoveParallel([Optional] string rawFile, [Optional] string destinationPath)
        {
            var inputFiles = Directory.GetFiles(_dirDestination);
            if (inputFiles.Count() != 0)
                foreach (var inFile in inputFiles)
                    File.Delete(inFile);

            Debug.WriteLine("Старт переименования и перемещения...");
            var swRename = new Stopwatch();
            swRename.Start();

            if (!string.IsNullOrEmpty(rawFile))
                _dirInput = rawFile;

            if (!string.IsNullOrEmpty(destinationPath))
                _dirDestination = destinationPath;

            string[] baseFolder = Directory.GetDirectories(_dirInput);

            Parallel.ForEach(baseFolder, dir =>
            {
                /// Тут можно упростить =>
                string[] subDir = Directory.GetDirectories(Path.Combine(dir, "files"));

                List<string> filesSubfolder = new List<string>();
                filesSubfolder.AddRange(Directory.GetFiles(Path.Combine(subDir[0], "xml")));

                foreach (string file in filesSubfolder)
                    Task.Run(() =>
                        File.Copy(file, Path.Combine(_dirDestination, string.Concat(Path.GetFileName(dir), ".", Path.GetFileName(file))))
                    );
            });
#if DEBUG
            swRename.Stop();
            _elapsedMilliseconds = swRename.ElapsedMilliseconds;
            _countRawFiles = baseFolder.Count();
#endif
            // Delete non usable base folder
            if (Config.DeleteSourceFiles)
                Task.Run(() => Delete(""));
        }

        //public static void GetLastStatistics()
        //{
        //    Console.WriteLine("{");
        //    Console.WriteLine("\tOperation => ParseXMLByMaskParallel is DONE!");
        //    Console.WriteLine($"\nTotal time: {_elapsedMilliseconds/1000},{_elapsedMilliseconds % 1000} msec");
        //    Console.WriteLine($"\tRaw folder with files: {_countRawFiles}");
        //    Console.WriteLine($"Total files ({Directory.GetFiles("C:\\_test\\inputFiles").Count()} " +
        //        $"/ {Directory.GetFiles("C:\\_test\\rawFiles").Count()})");
        //    Console.WriteLine($"AVG time: {Directory.GetFiles("C:\\_test\\inputFiles").Count() / (int)(_elapsedMilliseconds / 1000)},"
        //         + $"{_elapsedMilliseconds % 1000} units");
        //    Console.WriteLine("}\n");
        //}

        /// <summary> Удаление подпапок из исходной папки </summary>
        private void Delete(string inputDir)
        {
            foreach (var item in Directory.GetDirectories(inputDir))
                Directory.Delete(item);
        }

        /// <summary>Для внутренего использования</summary>
        /// <param name="lists">Массив с абсолютными путями к файлам</param>
        protected static void OutFilePath(List<string> lists)
        {
            foreach (var item in lists)
                Console.WriteLine($"[] => {item}");
        }
    }
}
