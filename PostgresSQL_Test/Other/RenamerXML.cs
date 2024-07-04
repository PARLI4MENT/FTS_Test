#define TEST1

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

        private static string _dirInput = Path.Combine(_rootDir, "ParseInput");

        private static string _dirDestination = Path.Combine(_rootDir, "ParseOutput");

        // Убрать в отдельный метод или класс
        /// <summary>
        /// Parse xml`s files by mask
        /// </summary>
        public void ParseFileByMasked()
        {
            // Timer
            var sw = new Stopwatch();
            sw.Start();

            var baseFolder = new List<string>();
            baseFolder.AddRange(Directory.GetDirectories(_dirInput).ToList<string>());

#if TEST
            int index = 0;
#endif
            int subFolder = 0;
            foreach (var dir in baseFolder)
            {
#if TEST
                Console.WriteLine($"[{index}] => {dir}");
#endif

                /// Тут можно упростить => 
                string tmpSubfolder = Path.GetFileName(dir);
                string[] subDir = Directory.GetDirectories(Path.Combine(dir, "files"));

                List<string> filesSubfolder = new List<string>();
                filesSubfolder.AddRange(Directory.GetFiles(Path.Combine(subDir[0], "xml")));
#if TEST
#endif
                foreach (string file in filesSubfolder)
                {
#if TEST
                    Console.WriteLine($"\t[{subIndex}] => {Path.GetFileName(file)}");
#endif
                    subFolder += filesSubfolder.Count;
                    Task.Run(() =>
                        File.Copy(file, Path.Combine(_dirDestination, string.Concat(tmpSubfolder, ".", Path.GetFileName(file)))));
#if TEST
                    subIndex++;
#endif
                }
            }

            // Timer
            sw.Stop();

            Console.WriteLine("{");
            Console.WriteLine("\tOperation => ParseXMLByMask is DONE!");
            Console.WriteLine($"\tBase folder = {baseFolder.Count}");
            Console.WriteLine($"\tParsed files = {subFolder}");
            Console.WriteLine($"\tTotal time: [{sw.Elapsed}]");
            Console.WriteLine("{\n");

            // Delete non usable base folder
            Task.Run(() => Delete());
        }

        /// <summary>
        /// Parallel parse xml`s files by mask
        /// </summary>
        public void ParseFileByMaskedParallel()
        {
            // Timer
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            // Переписать на string[]
            var baseFolder = new List<string>();
            baseFolder.AddRange(Directory.GetDirectories(_dirInput).ToList<string>());

#if TEST
            int index = 0;
#endif
            int subFolder = 0;
            Parallel.ForEach(baseFolder, dir =>
            {
#if TEST
                Console.WriteLine($"[{index}] => {dir}");
#endif

                /// Тут можно упростить => 
                string tmpSubfolder = Path.GetFileName(dir);
                string[] subDir = Directory.GetDirectories(Path.Combine(dir, "files"));

                List<string> filesSubfolder = new List<string>();
                filesSubfolder.AddRange(Directory.GetFiles(Path.Combine(subDir[0], "xml")));


                foreach (string file in filesSubfolder)
                {
#if TEST
                    Console.WriteLine($"\t[{subIndex}] => {Path.GetFileName(fie)}");
#endif
                    subFolder += filesSubfolder.Count;
                    Task.Run(() =>
                        File.Copy(file, Path.Combine(_dirDestination, string.Concat(tmpSubfolder, ".", Path.GetFileName(file))))
                    );
                    //RenameFile(file, tmpSubfolder);
#if TEST
                    subIndex++;
#endif
                }
            });

            // Timer
            sw.Stop();

            Console.WriteLine("{");
            Console.WriteLine("\tOperation => ParseXMLByMaskParallel is DONE!");
            Console.WriteLine($"\tBase folder = {baseFolder.Count}");
            Console.WriteLine($"\tParsed files = {subFolder}");
            Console.WriteLine($"\tTotal time: [{sw.Elapsed}]");
            Console.WriteLine("}\n");

            // Delete non usable base folder
            //Task.Run(() => Delete());
        }

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
        public static void OutFilePath(List<string> lists)
        {
            foreach (object item in lists)
            {
                Console.WriteLine($"[] => {item}");
            }
        }
    }
}
