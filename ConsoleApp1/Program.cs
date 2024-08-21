using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Runtime.Remoting;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ReadAllSetting();



            Console.WriteLine();
            Console.ReadKey();
        }

        static void ReadAllSetting()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings != null )
                    Console.WriteLine("AppSettings is empty.");
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        Console.WriteLine($@"Key={key} => {appSettings[key]}");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }
        }

        static void ReadSetting(string key)
        {
            try
            {

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }
        }
    }
}
