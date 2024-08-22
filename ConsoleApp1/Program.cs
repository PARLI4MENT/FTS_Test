using System;
using System.Configuration;

namespace ConfigurationTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ReadAllSetting();

            Console.WriteLine();
            Console.ReadKey();
        }

        public static void ReadAllSetting()
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
            catch (ConfigurationErrorsException ex) { Console.WriteLine(ex.Message); return; }
        }

        public static void ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "Not found";
                Console.WriteLine(result);
            }
            catch (ConfigurationErrorsException ex) { Console.WriteLine(ex.Message); return; }
        }

        public static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] is null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException ex) { Console.WriteLine(ex.Message); return; }
        }
    }
}
