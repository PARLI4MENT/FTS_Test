#define DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using XmlFTS.OutClass;

namespace XmlFTS
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
        public static string RenameMoveRawFiles(string[] xmlFiles)
        {
            foreach (var xmlFile in xmlFiles)
            {
                string code = Path.GetFileName(Path.GetDirectoryName(xmlFile));

                if (!Directory.Exists(Path.Combine("C:\\_2\\ExtractionFiles")))
                    Directory.CreateDirectory(Path.Combine("C:\\_2\\ExtractionFiles"));

                if (File.Exists(xmlFile))
                {
                    string tmpPathCombine = Path.Combine("C:\\_2\\ExtractionFiles", string.Concat(code, ".", Path.GetFileName(xmlFile)));
                    if (!File.Exists(tmpPathCombine))
                    {
                        File.Copy(xmlFile, tmpPathCombine, true);
                        return tmpPathCombine;
                    }
                }
            }
            return null;
        }

        public static string RenameMoveRawFiles(string xmlFiles)
        {
            string code = Path.GetFileName(Path.GetDirectoryName(xmlFiles));

            if (!Directory.Exists(Path.Combine("C:\\_2\\ExtractionFiles")))
                Directory.CreateDirectory(Path.Combine("C:\\_2\\ExtractionFiles"));

            if (File.Exists(xmlFiles))
            {
                string tmpPathCombine = Path.Combine("C:\\_2\\ExtractionFiles", string.Concat(code, ".", Path.GetFileName(xmlFiles)));
                if (!File.Exists(tmpPathCombine))
                {
                    File.Copy(xmlFiles, tmpPathCombine, true);
                    return tmpPathCombine;
                }
            }
            return null;
        }

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
