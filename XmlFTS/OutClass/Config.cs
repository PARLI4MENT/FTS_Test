using System.Configuration;
using System.Diagnostics;

namespace XmlFTS.OutClass
{
    static class Config
    {

        /// <summary> Инициализация базовой конфигурация. Пути к папкам, шаблонам итд. </summary>
        public static void BaseConfiguration()
        {

        }

        /// <summary> Выводит все ключи и значения настроек в Debug консоли </summary>
        public static void ReadAllSetting()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings != null)
                    Debug.WriteLine("Application setting is empty");
                else
                    foreach (var key in appSettings.AllKeys)
                        Debug.WriteLine($"Key: {key} => {appSettings[key]}");
            }
            catch (ConfigurationErrorsException ex) { Debug.WriteLine(ex.Message); return; }
        }

        public static string ReadSettings(string key)
        {
            try
            {
                var appSetting = ConfigurationManager.AppSettings;
                string result = appSetting[key] ?? "Not found";
                Debug.WriteLine(result);
                return result;
            }
            catch (ConfigurationErrorsException ex) { Debug.WriteLine(ex.Message); return null; }
        }

        /// <summary> Добавление/обновление настройки  </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] is null)
                    settings.Add(key, value);
                else
                    settings[key].Value = value;
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException ex) { Debug.WriteLine(ex.Message); return; }
        }
    }
}