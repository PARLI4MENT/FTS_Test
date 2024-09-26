#define TEST
///5.23.0/3.4.16

using SQLNs;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Threading;
using XmlFTS;
using XmlFTS.OutClass;

namespace XMLSigner
{
    public static class Program
    {       
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            //new PgSql().PgRetriveData("BD2D10AB-2871-4155-8F0A-2CE896EA880F", "BD2D10AB-2871-4155-8F0A-2CE896EA880F", "Общая ошибка при работе системы");
            Config.BaseConfiguration("C:\\Test");
            Config.EnableBackup = false;
            Config.DeleteSourceFiles = true;
            Config.ReadAllSetting();

            await ProcessHostXML.RunProcess();
            //new PgSql().PgRetriveData("BD2D10AB-2871-4155-8F0A-2CE896EA880F", "BD2D10AB-2871-4155-8F0A-2CE896EA880F", "Общая ошибка при работе системы");

            //new PgSql().PgSqlCreateDatabase(true);

            //TemplatingXml.CreateArchive(MchdId, INN, cert);
        }
    }
}