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
            Config.BaseConfiguration("C:\\_test");
            Config.DeleteSourceFiles = false;
            Console.WriteLine();

            SignXmlGost.FindGostCurrentCertificate(string.Empty);

            //XmlNs.ImplementateToXml.ImplementLinear("C:\\_test\\intermidateFiles\\2a7f4ca8-ae0a-454c-9091-e915f15879ae.filesList.xml");

            Console.Write("\nPress any key...");
            Console.ReadKey();
            Console.ReadKey();
        }

        private static void ProcessStart()
        {
            /// Базовая инициализация конфигурации
            Config.BaseConfiguration("C:\\_test");
            Config.DeleteSourceFiles = false;
            Console.WriteLine();

            var rawSrcFolder = Directory.GetDirectories(StaticPathConfiguration.PathRawFolder);

            Parallel.ForEach(rawSrcFolder,
                new ParallelOptions { MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism },
                rawFolder =>
                {
                    string[] xmlFiles = Directory.GetFiles(rawFolder, "*.xml", SearchOption.AllDirectories);

                    foreach (string xmlFile in xmlFiles)
                    {
                        // Combine Xml
                        string tmpPathCombine = Path.Combine(StaticPathConfiguration.PathIntermidateFolder, string.Concat(Path.GetFileName(rawFolder), ".", Path.GetFileName(xmlFile)));
                        File.Copy(xmlFile, tmpPathCombine, true);


                        /// Implement
                        //var tempImplement = XmlNs.ImplementateToXml.ImplementLinear(tmpPathCombine);

                    }
                });
            Console.WriteLine();
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