using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace SqlTest
{
    public class RenamerXML
    {
        [NotNull]
        static string _rootDir = "C:\\_test\\";
        public static string RootDir { get { return _rootDir; } set { _rootDir = value; } }

        private static string _dirInput = Path.Combine(_rootDir, "arch");

        private static string _dirDestination = Path.Combine(_rootDir, "OUT");

        // Убрать в отдельный метод или класс
        /// <summary>
        /// Parse xml`s files by mask
        /// </summary>
        public static void ParseFileByMasked()
        {
            var sw = new Stopwatch();
            sw.Start();

            var baseFolder = new List<string>();
            baseFolder.AddRange(Directory.GetDirectories(_dirInput).ToList<string>());

#if DEBUG
            int index = 0;
#endif
            foreach (var dir in baseFolder)
            {
#if DEBUG
                Console.WriteLine($"[{index}] => {dir}");
#endif

                /// Тут можно упростить => 
                string tmpSubfolder = Path.GetFileName(dir);
                string[] subDir = Directory.GetDirectories(Path.Combine(dir, "files"));

                List<string> filesSubfolder = new List<string>();
                filesSubfolder.AddRange(Directory.GetFiles(Path.Combine(subDir[0], "xml")));
#if DEBUG
                int subIndex = 0;
#endif
                foreach (string file in filesSubfolder)
                {
#if DEBUG
                            Console.WriteLine($"\t[{subIndex}] => {Path.GetFileName(file)}");
#endif
                    Task.Run(() => RenameFile(file, tmpSubfolder));
                    //RenameFile(file, tmpSubfolder);
#if DEBUG
                    subIndex++;
#endif
                }
            }

            sw.Stop();
            Console.WriteLine("\nOperation => ParseXMLByMaskParallel is DONE!");
            Console.WriteLine($@"Total time: [{sw.Elapsed}]");
        }

        /// <summary>
        /// Parallel parse xml`s files by mask
        /// </summary>
        public static void ParseFileByMaskedParallel()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var baseFolder = new List<string>();
            baseFolder.AddRange(Directory.GetDirectories(_dirInput).ToList<string>());

#if DEBUG
            int index = 0;
#endif
            Parallel.ForEach(baseFolder, dir =>
            {
#if DEBUG
                Console.WriteLine($"[{index}] => {dir}");
#endif

                /// Тут можно упростить => 
                string tmpSubfolder = Path.GetFileName(dir);
                string[] subDir = Directory.GetDirectories(Path.Combine(dir, "files"));

                List<string> filesSubfolder = new List<string>();
                filesSubfolder.AddRange(Directory.GetFiles(Path.Combine(subDir[0], "xml")));

#if DEBUG
                int subIndex = 0;
#endif

                foreach (string file in filesSubfolder)
                {
#if DEBUG
                    Console.WriteLine($"\t[{subIndex}] => {Path.GetFileName(file)}");
#endif

                    Task.Run(() => RenameFile(file, tmpSubfolder));
                    //RenameFile(file, tmpSubfolder);
#if DEBUG
                    subIndex++;
#endif
                }
            });

            sw.Stop();
            Console.WriteLine("\nOperation => ParseXMLByMaskParallel is DONE!");
            Console.WriteLine($@"Total time: [{sw.Elapsed}]");
        }

        /// <summary>
        /// Move and rename targer file
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <param name="subName"></param>
        /// <returns></returns>
        private static string RenameFile(string pathToFile, string subName)
        {

#if DEBUG
            Console.WriteLine($"\t\t[Base path] => " +
                $"{Path.Combine(Path.GetDirectoryName(pathToFile), Path.GetFileName(pathToFile))}");
#endif
            string tmp = Path.Combine(_dirDestination, string.Concat(subName, ".", Path.GetFileName(pathToFile)));
            File.Move(pathToFile, tmp);

#if DEBUG
            Console.WriteLine($"\t\t[Dest. path] => {tmp}");
#endif
            return tmp;
        }

        public static void OutFilePath(List<string> lists)
        {
            foreach (object item in lists)
            {
                Console.WriteLine($"[] => {item}");
            }
        }
    }
}
