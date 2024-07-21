using GostCryptography.Asn1.Gost.Gost_R3410_2001;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Schema;
using System.Configuration;

namespace SQLTestNs
{
    public class Conifiguration
    {

        public static void BaseInitilize(string rootFolder)
        {
            if (Directory.Exists(rootFolder))
            {

            }
            else
            {
                Console.WriteLine();
            }
        }

        public static void FolderStructInitialize(string rootFolder)
        {
            if (Directory.Exists(rootFolder))
            {


                //Properties.Settings.Default.Save();
            }
        }

        private bool CheckTemplate()
        {
            if (File.Exists(Path.Combine()))
            {
                Debug.WriteLine("Файл шаблона существует");
                return true;

            }
            Debug.WriteLine("Файл шаблона не найден!");
            return false;
        }
    }
}
   