using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgTest
{
    public class WorkerXML
    {
        static string dirInput = "C:\\workPath\\cellog";
        static string dirDestination = "C:\\PathDestination";

        public WorkerXML() { }

        public void ParseFileByMasked(string basePath)
        {
            if (CheckExistsInputPath())
            {
                var baseFolder = new List<string>();
                baseFolder.AddRange(Directory.GetDirectories(dirInput).ToList<string>());

#if DEBUG
                int index = 0;
#endif
                foreach (var dir in baseFolder)
                {
#if DEBUG
                    Console.WriteLine($"[{index}] => {dir}");
#endif
                    string tmpSubfolder = Path.GetFileName(dir);
                    string[] subDir = Directory.GetDirectories(Path.Combine(dir, "files"));
                    var endPath = Path.Combine(subDir[0], "xml");

                    List<string> filesSubfolder = new List<string>();
                    filesSubfolder.AddRange(Directory.GetFiles(endPath));

#if DEBUG
                    int subIndex = 0;
#endif
                    foreach (string file in filesSubfolder)
                    {
#if DEBUG
                        Console.WriteLine($"\t[{subIndex}] => {Path.GetFileName(file)}");
#endif

                        RenameFile(file, tmpSubfolder);
#if DEBUG
                        subIndex++;
#endif
                    }
                }
            }
        }

        private string RenameFile(string pathToFile, string subName)
        {
            string pathDestination = "C:\\_DestinationFolder";

#if DEBUG
            Console.WriteLine($"\t\t[Base path] => {Path.Combine(Path.GetDirectoryName(pathToFile), Path.GetFileName(pathToFile))}");
#endif
            string tmp = Path.Combine(pathDestination, string.Concat(subName, ".", Path.GetFileName(pathToFile)));
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

        public static bool CheckExistsInputPath()
        {
            if (!Directory.Exists(dirDestination) &&
                !Directory.Exists(dirInput))
            {
                Directory.CreateDirectory(dirDestination);
                Directory.CreateDirectory(dirInput);
                return true;
            }
            return true;
        }
    }
}
