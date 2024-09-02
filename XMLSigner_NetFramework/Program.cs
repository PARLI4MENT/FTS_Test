#define TEST

using FileNs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using XmlFTS.OutClass;

namespace XMLSigner
{
    public static class Program
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


            Console.Write("\nPress any key...");
            Console.ReadKey();
            Console.ReadKey();
        }

        private static void ProcessStart()
        {
            var rawSrcFolder = Directory.GetDirectories("C:\\Dekl\\SEND DATA");

            var sw = new Stopwatch();
            sw.Start();

            foreach (var rawFolder in rawSrcFolder)
            {
                /// #1 Extraction ZIP
                var listXml = ArchiveWorker.ExtractZipArchive(Directory.GetFiles(rawFolder, "*.zip")[0]);

                /// #2 Rename Move
                string[] xmlFiles = Directory.GetFiles(rawFolder, "*.xml");
                foreach (var xmlFile in xmlFiles)
                    listXml.Add(RenameMoveFileOnly(xmlFile));

                /// #3 Sort
                SortXml(listXml);
            }

            sw.Stop();
            Console.WriteLine();
            Console.WriteLine($"General => {sw.ElapsedMilliseconds / 1000} sec.");

            Console.WriteLine();
        }

        /// <summary>Переименование и перемещение</summary>
        /// <param name="pathRawFile">Путь к файлу</param>
        public static string RenameMoveFileOnly(string pathRawFile, string dirDestination = "C:\\_2\\ExtractionFiles")
        {
            string code = Path.GetFileName(Path.GetDirectoryName(pathRawFile));

            if (!Directory.Exists(Path.Combine(dirDestination)))
                Directory.CreateDirectory(Path.Combine(dirDestination));

            if (File.Exists(pathRawFile))
            {
                string tmpPathCombine = Path.Combine(dirDestination, string.Concat(code, ".", Path.GetFileName(pathRawFile)));
                if (!File.Exists(tmpPathCombine))
                {
                    File.Move(pathRawFile, tmpPathCombine);
                    return tmpPathCombine;
                }
            }
            return null;
        }

        public static void SortXml(List<string> filesXMl)
        {
            foreach (string xmlFile in filesXMl)
            {
                if (File.Exists(xmlFile))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(new StringReader(File.ReadAllText(xmlFile)));

                    switch (xmlDoc.DocumentElement.GetAttribute("DocumentModeID"))
                    {
                        /// ПТД ExpressCargoDeclaration
                        case "1006275E":
                            {
                                var tmpFilePath = Path.Combine("C:\\_2\\Sorted\\ptd", Path.GetFileName(xmlFile));
                                if (!File.Exists(tmpFilePath))
                                    File.Move(xmlFile, tmpFilePath);
                            }
                            break;

                        /// В архив Остальное
                        default:
                            {
                                var tmpFilePath = Path.Combine("C:\\_2\\Sorted\\toArchive", Path.GetFileName(xmlFile));
                                if (!File.Exists(tmpFilePath))
                                    File.Move(xmlFile, tmpFilePath);
                            }
                            break;
                    }
                }
            }
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