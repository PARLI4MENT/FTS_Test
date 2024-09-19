#define TEST
///5.23.0/3.4.16

using SQLNs;
using System;
using System.Configuration;
using XmlFTS;
using XmlFTS.OutClass;

namespace XMLSigner
{
    public static class Program
    {       
        public static void Main(string[] args)
        {
            Console.WriteLine();
            Config.BaseConfiguration("C:\\_2");
            Config.ReadAllSetting();
            Config.EnableBackup = false;
            Config.DeleteSourceFiles = true;
            Console.Clear();
            Config.ReadAllSetting();

            ProcessXML.ProcessStart();

            Console.ReadKey();
        }
    }
}