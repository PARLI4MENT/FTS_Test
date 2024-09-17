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
        public static void RenameMoveRawFiles(string[] xmlFiles)
        {
            foreach (var xmlFile in xmlFiles)
            {
                string code = Path.GetFileName(Path.GetDirectoryName(xmlFile));

                string tmpPathCombine = Path.Combine(StaticPathConfiguration.PathExtractionFolder, string.Concat(code, ".", Path.GetFileName(xmlFile)));
                
                if (!File.Exists(tmpPathCombine))
                {
                    File.Copy(xmlFile, tmpPathCombine, true);

                    if (Config.EnableBackup)
                        File.Copy(xmlFile, Path.Combine(StaticPathConfiguration.PathBackupFolder, string.Concat(code, ".", Path.GetFileName(xmlFile))), true);
                }
            }
        }

        public static void RenameMoveRawFiles(string xmlFile)
        {
            string code = Path.GetFileName(Path.GetDirectoryName(xmlFile));
            string tmpPathExtraction = Path.Combine(StaticPathConfiguration.PathExtractionFolder, string.Concat(code, ".", Path.GetFileName(xmlFile)));
            
            if (!File.Exists(tmpPathExtraction))
            {
                File.Copy(xmlFile, tmpPathExtraction, true);

                if (Config.EnableBackup)
                    File.Copy(xmlFile, Path.Combine(StaticPathConfiguration.PathBackupFolder, string.Concat(code, ".", Path.GetFileName(xmlFile))), true);
            }
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
                Debug.WriteLine($"[] => {item}");
        }
    }
}
