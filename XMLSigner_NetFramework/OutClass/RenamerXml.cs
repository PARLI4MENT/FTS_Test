#define TEST1

using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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

        public static void RenameMove(string[] rawFolders, int _MaxDegreeOfParallelism = -1)
        {
            foreach (var rawFolder in rawFolders)
            {
                string[] subDir = Directory.GetDirectories(Path.Combine(rawFolder, "files"));
                string[] filesSubfolder = (Directory.GetFiles(Path.Combine(subDir[0], "xml")));

                foreach (string file in filesSubfolder)
                {
                    // Combine Xml
                    string tmpPathCombine = Path.Combine("C:\\_test\\intermidateFiles", string.Concat(Path.GetFileName(rawFolder), ".", Path.GetFileName(file)));
                    File.Copy(file, tmpPathCombine);
                }
            }
        }

        public static void RenameMoveParallel(string[] rawFolders, int _MaxDegreeOfParallelism = -1)
        {
            if (_MaxDegreeOfParallelism < -1)
                _MaxDegreeOfParallelism = -1;

            Parallel.ForEach(rawFolders,
                new ParallelOptions { MaxDegreeOfParallelism = _MaxDegreeOfParallelism },
                rawFolder => {
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
        /// <param name="pathRawFiles">Путь к папке с сырыми Xml-файлами</param>
        /// <param name="_MaxDegreeOfParallelism">Максимальное ко-во параллельное вычисление</param>
        public static void RenameMoveParallel(string pathRawFiles, int _MaxDegreeOfParallelism = -1)
        {
            if (_MaxDegreeOfParallelism < -1)
                _MaxDegreeOfParallelism = -1;

            var rawFolder = Directory.GetDirectories(pathRawFiles);
            Parallel.ForEach(rawFolder,
                new ParallelOptions { MaxDegreeOfParallelism = _MaxDegreeOfParallelism },
                rawFile => {
                    string[] subDir = Directory.GetDirectories(Path.Combine(rawFile, "files"));
                    string[] filesSubfolder = (Directory.GetFiles(Path.Combine(subDir[0], "xml")));

                    foreach (string file in filesSubfolder)
                    {

                        // Combine Xml
                        string tmpPathCombine = Path.Combine("C:\\_test\\intermidateFiles", string.Concat(Path.GetFileName(rawFile), ".", Path.GetFileName(file)));
                        File.Copy(file, tmpPathCombine);
                    }
                });
        }

        /// <summary>
        /// Перемещение и переименование Xml-файлов по маске
        /// </summary>
        /// <param name="rawFile">Путь к папке с исходными файлами</param>
        /// <param name="destinationPath">Путь к папке назначения</param>
        /// <param name="deletedRawFolder">Удалять исходную папку</param>
        public void RenameAndMove([Optional] string rawFile, [Optional] string destinationPath, bool deletedRawFolder = false)
        {
            /// Clear InputFiles from inputFiles folder
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

            // Timer
            swRename.Stop();
            _elapsedMilliseconds = swRename.ElapsedMilliseconds;
            _countRawFiles = baseFolder.Count();

            Console.WriteLine("{\n");

            // Delete non usable base folder
            if (deletedRawFolder)
                Task.Run(() => Delete());
        }

        /// <summary>
        /// Паралельное перемещение и переименование Xml-файлов по маске
        /// </summary>
        /// <param name="rawFile">Путь к папке с исходными файлами</param>
        /// <param name="destinationPath">Путь к папке назначения</param>
        /// <param name="deletedRawFolder">Удалять исходную папку</param>
        public void RenameAndMoveParallel([Optional] string rawFile, [Optional] string destinationPath, bool deletedRawFolder = false)
        {
            /// Clear InputFiles from inputFiles folder
            {
                var inputFiles = Directory.GetFiles(_dirDestination);
                if (inputFiles.Count() != 0)
                    foreach (var inFile in inputFiles)
                        File.Delete(inFile);
            }

            Console.WriteLine("Старт переименования и перемещения...");
            var swRename = new Stopwatch();
            swRename.Start();

            if (!string.IsNullOrEmpty(rawFile))
                _dirInput = rawFile;

            if (!string.IsNullOrEmpty(destinationPath))
                _dirDestination = destinationPath;

            string[] baseFolder = Directory.GetDirectories(_dirInput);

            int subFolder = 0;
            Parallel.ForEach(baseFolder, dir =>
            {
                /// Тут можно упростить =>
                string[] subDir = Directory.GetDirectories(Path.Combine(dir, "files"));

                List<string> filesSubfolder = new List<string>();
                filesSubfolder.AddRange(Directory.GetFiles(Path.Combine(subDir[0], "xml")));


                foreach (string file in filesSubfolder)
                {
                    subFolder += filesSubfolder.Count;
                    Task.Run(() =>
                        File.Copy(file, Path.Combine(_dirDestination, string.Concat(Path.GetFileName(dir), ".", Path.GetFileName(file))))
                    );
                }
            });

            _subFolder = subFolder; ;

            Console.WriteLine($"\tParsed files = {subFolder}");

            swRename.Stop();
            _elapsedMilliseconds = swRename.ElapsedMilliseconds;
            _countRawFiles = baseFolder.Count();

            // Delete non usable base folder
            if (deletedRawFolder)
                Task.Run(() => Delete());
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

        /// <summary>
        /// Deleted raw folder
        /// </summary>
        private void Delete()
        {
            foreach (var item in Directory.GetDirectories(_dirInput))
            {
                Directory.Delete(item);
            }
        }

        /// <summary>
        /// Для личных нужд
        /// </summary>
        /// <param name="lists"></param>
        protected static void OutFilePath(List<string> lists)
        {
            foreach (object item in lists)
            {
                Console.WriteLine($"[] => {item}");
            }
        }
    }
}
