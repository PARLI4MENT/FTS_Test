#define TEST
///5.23.0/3.4.16

using SQLNs;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using XmlFTS;
using XmlFTS.OutClass;
using XMLSigner.OutClass;

namespace XMLSigner
{
    public static class Program
    {
        //private static string MchdId = "e7d94ee1-33d4-4b95-a27d-07896fdc00e0";
        //private static string MchdINN = "250908790897";
        private static string MchdId = "719f90af-f777-4c70-9a33-053958eacc65";
        private static string INN = "2536287574";
        private static X509Certificate2 cert = SignXmlGost.FindGostCurrentCertificate("01DA FCE9 BC8E 41B0 0008 7F5E 381D 0002");

        public static void Main(string[] args)
        {
            Console.WriteLine();

            Config.BaseConfiguration("C:\\Test");
            Config.EnableBackup = false;
            Config.DeleteSourceFiles = true;
            Config.ReadAllSetting();

            Console.WriteLine(DateTime.Now.ToString("H-mm-ss_dd.MM.yyyy"));

            //ProcessXML.ProcessStart();
            //new PgSql().PgRetriveData("BD2D10AB-2871-4155-8F0A-2CE896EA880F", "BD2D10AB-2871-4155-8F0A-2CE896EA880F", "Общая ошибка при работе системы");

            //new PgSql().PgSqlCreateDatabase(true);

            //TemplatingXml.CreateArchive(MchdId, INN, cert);

            Console.ReadKey();
        }

    }
}