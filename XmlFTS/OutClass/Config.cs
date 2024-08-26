using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace XmlFTS.OutClass
{
    public static class Config
    {
        public static int MaxDegreeOfParallelism {
            get { return Convert.ToInt32(ReadSettings("MaxDegreeOfParallelism")); }
            set { AddUpdateAppSettings("MaxDegreeOfParallelism", value.ToString()); }
        }

        public static bool DeleteSourceFiles
        {
            get { return Convert.ToBoolean(ReadSettings("DeleteSourceFiles")); }
            set { AddUpdateAppSettings("DeleteSourceFiles", value.ToString()); }
        }

        /// <summary>
        /// Инициализация базовой конфигурация. Пути к папкам, шаблонам итд.
        /// </summary>Инициализация базовой конфигурация. Пути к папкам, шаблонам итд.
        /// <remarks>
        /// Файл с XML шаблоном должен лежать в базовой папке и называться template.xml
        /// Если файл с шаблоном отсутсвует, то вызвается окно выбора файла, затем файл копируется в корневую базовую папку
        /// </remarks>
        /// <param name="basePath"> Путь к базовой папке, в которой будут располагаться необходимые папки для работы</param>
        public static void BaseConfiguration([Optional]string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
                basePath = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            /// Путь к папке с исходными файлами
            StaticPathConfiguration.PathRawFolder = Path.Combine(basePath, "rawFiles");
            if (!Directory.Exists(StaticPathConfiguration.PathRawFolder))
                Directory.CreateDirectory(StaticPathConfiguration.PathRawFolder);
            AddUpdateAppSettings("PathRawFolder", StaticPathConfiguration.PathRawFolder);

            /// Путь к папке с промежуточными файлами
            StaticPathConfiguration.PathIntermidateFolder = Path.Combine(basePath, "intermidateFiles");
            if (!Directory.Exists(StaticPathConfiguration.PathIntermidateFolder))
                Directory.CreateDirectory(StaticPathConfiguration.PathIntermidateFolder);
            AddUpdateAppSettings("PathIntermidateFolder", StaticPathConfiguration.PathIntermidateFolder);

            /// Путь к папке с шаблонными файлами
            StaticPathConfiguration.PathImplementFolder = Path.Combine(basePath, "implementFiles");
            if (!Directory.Exists(StaticPathConfiguration.PathImplementFolder))
                Directory.CreateDirectory(StaticPathConfiguration.PathImplementFolder);
            AddUpdateAppSettings("PathImplementFolder", StaticPathConfiguration.PathImplementFolder);

            /// Путь к папке с подписанными файлами
            StaticPathConfiguration.PathSignedFolder = Path.Combine(basePath, "signedFiles");
            if (!Directory.Exists(StaticPathConfiguration.PathSignedFolder))
                Directory.CreateDirectory(StaticPathConfiguration.PathSignedFolder);
            AddUpdateAppSettings("PathSignedFolder", StaticPathConfiguration.PathSignedFolder);

            /*
            /// Определение файла шаблона
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = basePath;
            //openFileDialog.Filter = "Файл XML (.xml)|*.xml";
            //openFileDialog.FilterIndex = 0;
            //openFileDialog.Multiselect = false;
            //openFileDialog.RestoreDirectory = true;

            //var result = openFileDialog.ShowDialog();
            //if (result == true)
            //{
            //    string destFilename = Path.Combine(basePath, Path.GetFileName(openFileDialog.FileName));
            //    File.Copy(openFileDialog.FileName, destFilename);
            //    StaticPathConfiguration.TemplateXML = destFilename;
            //}
            */

            // Путь к файлу шаблоном
            if (!File.Exists(Path.Combine(basePath, "template.xml")))
                Debug.WriteLine("Файла с шаблоном не существует");
            StaticPathConfiguration.TemplateXML = Path.Combine(basePath, "template.xml");
            AddUpdateAppSettings("TemplateXML", StaticPathConfiguration.TemplateXML);

            MaxDegreeOfParallelism = 4;
            DeleteSourceFiles = false;

#if DEBUG
            Debug.WriteLine(string.Empty);
#endif
        }

        public static void BaseConfiguration(string PathRawFolder, string PathIntermidateFolder, string PathImplementFolder, string PathSignedFolder, string TemplateXML)
        {
            /// Путь к папке с исходными файлами
            if (!Directory.Exists(PathRawFolder))
                Directory.CreateDirectory(PathRawFolder);
            StaticPathConfiguration.PathRawFolder = PathRawFolder;
            AddUpdateAppSettings("PathRawFolder", StaticPathConfiguration.PathRawFolder);

            /// Путь к папке с промежуточными файлами
            if (!Directory.Exists(PathIntermidateFolder))
                Directory.CreateDirectory(PathIntermidateFolder);
            StaticPathConfiguration.PathIntermidateFolder = PathIntermidateFolder;
            AddUpdateAppSettings("PathIntermidateFolder", StaticPathConfiguration.PathIntermidateFolder);

            /// Путь к папке с шаблонными файлами
            if (!Directory.Exists(PathImplementFolder))
                Directory.CreateDirectory(PathImplementFolder);
            StaticPathConfiguration.PathImplementFolder = PathImplementFolder;
            AddUpdateAppSettings("PathImplementFolder", StaticPathConfiguration.PathImplementFolder);

            /// Путь к папке с подписанными файлами
            if (!Directory.Exists(PathSignedFolder))
                Directory.CreateDirectory(PathSignedFolder);
            StaticPathConfiguration.PathSignedFolder = PathSignedFolder;
            AddUpdateAppSettings("PathSignedFolder", StaticPathConfiguration.PathSignedFolder);

            // Путь к файлу шаблоном
            if (!File.Exists(TemplateXML))
                Debug.WriteLine("Файла с шаблоном не существует");
            StaticPathConfiguration.TemplateXML = TemplateXML;
            AddUpdateAppSettings("TemplateXML", StaticPathConfiguration.TemplateXML);
        }

        /// <summary> Выводит все ключи и значения настроек в Debug консоли </summary>
        public static void ReadAllSetting()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings != null)
                    Debug.WriteLine("Настройки приложения пусты");
                else
                    foreach (var key in appSettings.AllKeys)
                        Debug.WriteLine($"Key: {key} => {appSettings[key]}");
            }
            catch (ConfigurationErrorsException ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return;
            }
        }


        public static string ReadSettings(string key)
        {
            try
            {
                var appSetting = ConfigurationManager.AppSettings;
                string result = appSetting[key] ?? string.Empty;
                return result;
            }
            catch (ConfigurationErrorsException ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return null;
            }
        }

        /// <summary> Добавление/обновление поля конфигурации </summary>
        /// <param name="key">Наименование поля</param>
        /// <param name="value">Значение поля</param>
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
            catch (ConfigurationErrorsException ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return;
            }
        }

        /// <summary> Получение пути к файлу конфигурации </summary>
        public static string GetAppConfigLocation
        {
            get
            {
                return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            }
        }
    }
}