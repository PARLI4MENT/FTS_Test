#define TEST

using FileNs;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using XmlFTS.OutClass;

namespace XMLSigner
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            /// Открытие файла
            /*
            if (File.Exists(Config.GetAppConfigLocation))
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    Arguments = Path.GetDirectoryName(Config.GetAppConfigLocation),
                    FileName = Path.GetFileName(Config.GetAppConfigLocation)
                };
                Process.Start(processStartInfo);
            }
            Console.WriteLine();
            */
            
            ProcessStart();
            Console.WriteLine();

            /// Поиск сертификата
            //SignXmlGost.FindGostCurrentCertificate(string.Empty);

            //XmlNs.ImplementateToXml.ImplementLinear("C:\\_test\\intermidateFiles\\2a7f4ca8-ae0a-454c-9091-e915f15879ae.filesList.xml");

            Console.Write("\nPress any key...");
            Console.ReadKey();
            Console.ReadKey();
        }

        private static void ProcessStart()
        {
            var rawSrcFolder = Directory.GetDirectories("C:\\Dekl\\SEND DATA");

            Parallel.ForEach(rawSrcFolder,
                new ParallelOptions { MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism },
                rawFolder =>
                {
                    /// #1 Extraction ZIP
                    Console.WriteLine();
                    ArchiveWorker.ExtractZipArchive(Directory.GetFiles(rawFolder, "*.zip")[0], "");

                    /// #2 Rename Move
                    RenameMoveFileOnly(rawFolder, "");


                    /// #3
                    Console.WriteLine();
                });

            Console.WriteLine();
        }

        /// <summary>Переименование и перемещение</summary>
        /// <param name="pathRawFile">Путь к файлу</param>
        public static void RenameMoveFileOnly(string pathRawFile, string dirDestination = "C:\\_2\\ExtractionFiles")
        {
            string code = Path.GetDirectoryName(pathRawFile);
            if (!Directory.Exists(Path.Combine(dirDestination, code)))
                Directory.CreateDirectory(Path.Combine(dirDestination, code));

            if (File.Exists(pathRawFile))
            {
                string tmpPathCombine = Path.Combine(dirDestination, string.Concat(code, ".", Path.GetFileName(pathRawFile)));
                File.Copy(pathRawFile, tmpPathCombine, true);
            }

            return;
        }

        /// <summary> Возвращает xml-элементы включая входящий элемент, с его дочерними элементами в виде древа </summary>
        /// <remarks> </remarks>
        /// <param name="element"></param>
        public static void GetTree(XmlElement element)
        {
            if (element.GetType().Equals(typeof(XmlElement)))
            {
                foreach (var node in element.ChildNodes)
                {
                    if (node.GetType().Equals(typeof(XmlElement)))
                    {
                        var elem = (XmlElement)node;
                        Console.WriteLine($"\t{elem.Name}");
                        if (elem.InnerText == "" && elem.InnerText == null)
                            GetTree(elem);
                    }
                }
            }
        }
    }
}