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

            ProcessXML.StartProcess();

            Console.Write("\nPress any key...");
            Console.ReadKey();
        }
    }
}