#define TEST
///5.23.0/3.4.16

using SQLNs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using XmlFTS;
using XmlFTS.OutClass;

namespace XMLSigner
{
    public static class Program
    {
        private static string MchdId = "63afc9e5-db8e-46b9-9ee3-071df25b3eba".ToLower();
        private static string INN = "2536331150";

        private static X509Certificate2 cert = SignXmlGost.FindGostCurrentCertificate("01DA FCE9 BC8E 41B0 0008 7F5E 381D 0002");

        public static void Main(string[] args)
        {
            //new PgSql().PgRetriveData("BD2D10AB-2871-4155-8F0A-2CE896EA880F", "BD2D10AB-2871-4155-8F0A-2CE896EA880F", "Общая ошибка при работе системы");
            Console.WriteLine();

            Config.BaseConfiguration("C:\\Test");
            Config.EnableBackup = false;
            Config.DeleteSourceFiles = true;
            Config.ReadAllSetting();


            var rawSrcFolders = Directory.GetDirectories("C:\\Test\\RawFolder");

            int SummaryFiles = 0;

            if (rawSrcFolders != null)
            {
                foreach (var rawSrcFolder in rawSrcFolders)
                {
                    /// #1 Извлечение ZIP
                    foreach (string zipArchive in Directory.GetFiles(rawSrcFolder, "*.zip"))
                        ArchiveWorker.ExtractZipArchive(zipArchive);

                    /// #2 Переименование и копирование
                    string[] xmlFiles = Directory.GetFiles(rawSrcFolder, "*.xml");
                    if (xmlFiles.Count() == 1)
                        RenamerXML.RenameMoveRawFiles(xmlFiles[0]);
                    if (xmlFiles.Count() > 1)
                        RenamerXML.RenameMoveRawFiles(xmlFiles);

                    SummaryFiles += Directory.GetFiles(StaticPathConfiguration.PathExtractionFolder, "*.xml").Count();
                    /// Remove srcFolder
                    Directory.Delete(rawSrcFolder, true);

                }
            }
            Console.WriteLine($@"Count: {SummaryFiles}");
            Console.ReadKey();
        }

    }
}