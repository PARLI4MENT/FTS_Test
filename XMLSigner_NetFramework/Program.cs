#define TEST
///5.23.0/3.4.16

using SQLNs;
using System;
<<<<<<< HEAD
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
=======
using System.Security.Cryptography.X509Certificates;
>>>>>>> a7019b1c9a632e35e9b78a20cd1f75d39a2bcb55
using XmlFTS;
using XmlFTS.OutClass;

namespace XMLSigner
{
    public static class Program
    {
<<<<<<< HEAD
        /// <summary> Это нужно будет удалить </summary>
        private static string MchdId = "e7d94ee1-33d4-4b95-a27d-07896fdc00e0";
        private static string MchdINN = "250908790897";
=======
        private static string MchdId = "63afc9e5-db8e-46b9-9ee3-071df25b3eba".ToLower();
        private static string INN = "2536331150";

>>>>>>> a7019b1c9a632e35e9b78a20cd1f75d39a2bcb55
        private static X509Certificate2 cert = SignXmlGost.FindGostCurrentCertificate("01DA FCE9 BC8E 41B0 0008 7F5E 381D 0002");

        public static void Main(string[] args)
        {
            Console.WriteLine();

            Config.BaseConfiguration("C:\\Test");
            Config.EnableBackup = false;
            Config.DeleteSourceFiles = true;
            Config.ReadAllSetting();

<<<<<<< HEAD
            //ProcessXML.ProcessStart();
            //new PgSql().PgRetriveData("BD2D10AB-2871-4155-8F0A-2CE896EA880F", "BD2D10AB-2871-4155-8F0A-2CE896EA880F", "Общая ошибка при работе системы");

            var rawSrcFolders = Directory.GetDirectories(StaticPathConfiguration.PathRawFolder);

            if (rawSrcFolders != null)
                foreach (var rawSrcFolder in rawSrcFolders)
                {
                    Debug.WriteLine("Start main process...");

                    int SummaryFiles = 0;

                    /// #1 Извлечение ZIP
                    ArchiveWorker.ExtractZipArchive(Directory.GetFiles(rawSrcFolder, "*.zip")[0]);

                    /// #2 Переименование и копирование
                    string[] xmlFiles = Directory.GetFiles(rawSrcFolder, "*.xml");
                    if (xmlFiles.Count() == 1)
                        RenamerXML.RenameMoveRawFiles(xmlFiles[0]);
                    if (xmlFiles.Count() > 1)
                        RenamerXML.RenameMoveRawFiles(xmlFiles);

                    /// Remove srcFolder
                    Directory.Delete(rawSrcFolder, true);

                    ///// #3 Сортировка
                    SortXml(Directory.GetFiles(StaticPathConfiguration.PathExtractionFolder, "*.xml"));
                }

            Console.WriteLine("Process main done!");
            Console.ReadKey();
        }

        private static void SortXml(string[] xmlFiles)
        {
            if (xmlFiles == null)
                return;

            Parallel.ForEach
                (xmlFiles,
                new ParallelOptions { MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism },
                xmlFile =>
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(new StringReader(File.ReadAllText(xmlFile)));

                    switch (xmlDoc.DocumentElement.GetAttribute("DocumentModeID"))
                    {
                        /// ПТД ExpressCargoDeclaration
                        case "1006275E":
                            // Шаблонизация + выбрать серификат Конкретного человека
                            TemplatingXml.TemplatingLinear(xmlFile, ref cert, MchdId, MchdINN);
                            break;

                        /// В архив Остальное
                        default:
                            // Шаблонизация + выбрать серификат (Компании) ///Пока индивидуальный
                            TemplatingXml.TemplatingLinear(xmlFile, ref cert, MchdId, MchdINN);
                            break;
                    }
                });
        }
=======
            Console.WriteLine(DateTime.Now.ToString("H-mm-ss_dd.MM.yyyy"));

            //ProcessXML.ProcessStart();
            //new PgSql().PgRetriveData("BD2D10AB-2871-4155-8F0A-2CE896EA880F", "BD2D10AB-2871-4155-8F0A-2CE896EA880F", "Общая ошибка при работе системы");

            //new PgSql().PgSqlCreateDatabase(true);

            TemplatingXml.CreateArchive(MchdId, INN, cert);

            Console.ReadKey();
        }

>>>>>>> a7019b1c9a632e35e9b78a20cd1f75d39a2bcb55
    }
}