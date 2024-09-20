using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XmlFTS.OutClass
{
    public static class Config
    {

        /// <summary> Настройка => Максимальное кол-во потоков </summary>
        /// <remarks>По-умолчанию => 2</remarks>
        public static int MaxDegreeOfParallelism
        {
            //get { return Convert.ToInt32(ReadSettings("MaxDegreeOfParallelism")); }
            get { return 2; }
            set { AddUpdateAppSettings("MaxDegreeOfParallelism", value.ToString()); }
        }

        /// <summary> Настройка => Удалять исходные файлы </summary>
        /// <remarks>По-умолчанию => true</remarks>
        public static bool DeleteSourceFiles
        {
            //get { return Convert.ToBoolean(ReadSettings("DeleteSourceFiles")); }
            get { return true; }
            set { AddUpdateAppSettings("DeleteSourceFiles", value.ToString()); }
        }

        /// <summary> Настройка => Делать резеврную копию </summary>
        /// <remarks>не доделано</remarks>
        public static bool EnableBackup
        {
            //get { return Convert.ToBoolean(ReadSettings("BackupEnable")); }
            get { return false; }
            set { AddUpdateAppSettings("BackupEnable", value.ToString()); }
        }

        /// <summary>
        /// Инициализация базовой конфигурация. Пути к папкам, шаблонам итд.
        /// </summary>Инициализация базовой конфигурация. Пути к папкам, шаблонам итд.
        /// <remarks>
        /// Файл с XML шаблоном должен лежать в базовой папке и называться template.xml
        /// </remarks>
        /// <param name="basePath"> Путь к базовой папке, в которой будут располагаться необходимые папки для работы</param>
        /// <param name="deleteFile"> Удалять файлы после обработки </param>
        public static void BaseConfiguration([Optional]string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
                basePath = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            /// Путь к папке с исходными файлами
            StaticPathConfiguration.PathRawFolder = Path.Combine(basePath, "RawFolder");
            if (!Directory.Exists(StaticPathConfiguration.PathRawFolder))
                Directory.CreateDirectory(StaticPathConfiguration.PathRawFolder);
            AddUpdateAppSettings("PathRawFolder", StaticPathConfiguration.PathRawFolder);

            /// Путь к папке с извлечёнными файлами
            StaticPathConfiguration.PathExtractionFolder = Path.Combine(basePath, "ExtractionFolder");
            if (!Directory.Exists(StaticPathConfiguration.PathExtractionFolder))
                Directory.CreateDirectory(StaticPathConfiguration.PathExtractionFolder);
            AddUpdateAppSettings("PathExtractionFolder", StaticPathConfiguration.PathExtractionFolder);

            /// Путь к папке с шаблонными файлами
            StaticPathConfiguration.PathTemplatedFolder = Path.Combine(basePath, "TemplatedFolder");
            if (!Directory.Exists(StaticPathConfiguration.PathTemplatedFolder))
                Directory.CreateDirectory(StaticPathConfiguration.PathTemplatedFolder);
            AddUpdateAppSettings("PathTemplatedFolder", StaticPathConfiguration.PathTemplatedFolder);

            /// Путь к папке с подписанными файлами
            StaticPathConfiguration.PathSignedFolder = Path.Combine(basePath, "SignedFolder");
            if (!Directory.Exists(StaticPathConfiguration.PathSignedFolder))
                Directory.CreateDirectory(StaticPathConfiguration.PathSignedFolder);
            AddUpdateAppSettings("PathSignedFolder", StaticPathConfiguration.PathSignedFolder);

            /// Путь к папке с резервными файлами
            StaticPathConfiguration.PathBackupFolder = Path.Combine(basePath, "BackupFolder");
            if (!Directory.Exists(StaticPathConfiguration.PathBackupFolder))
                Directory.CreateDirectory(StaticPathConfiguration.PathBackupFolder);
            AddUpdateAppSettings("PathBackupFolder", StaticPathConfiguration.PathBackupFolder);

            /// Путь к папке с принимаемыми файлами от ФТС
            StaticPathConfiguration.PathReplyFTS = Path.Combine(basePath, "ReplyFTS");
            if (!Directory.Exists(StaticPathConfiguration.PathReplyFTS))
                Directory.CreateDirectory(StaticPathConfiguration.PathReplyFTS);
            AddUpdateAppSettings("PathReplyFTS", StaticPathConfiguration.PathReplyFTS);

            // Путь к файлу шаблоном
            if (!File.Exists(Path.Combine(basePath, "template.xml")))
                Debug.WriteLine("Файла с шаблоном не существует");
            StaticPathConfiguration.TemplateXML = Path.Combine(basePath, "template.xml");
            AddUpdateAppSettings("TemplateXML", StaticPathConfiguration.TemplateXML);
        }

        public static void BaseConfiguration(string PathRawFolder, string PathExtractionFolder, string PathIntermidateFolder, string PathTemplatedFolder, string PathSignedFolder, string PathBackupFolder, string PathReplyFTS, string TemplateXML)
        {
            /// Путь к папке с исходными файлами
            if (!Directory.Exists(PathRawFolder))
                Directory.CreateDirectory(PathRawFolder);
            StaticPathConfiguration.PathExtractionFolder = PathRawFolder;
            AddUpdateAppSettings("PathRawFolder", StaticPathConfiguration.PathRawFolder);

            /// Путь к папке с извлечёнными файлами
            if (!Directory.Exists(PathExtractionFolder))
                Directory.CreateDirectory(PathExtractionFolder);
            StaticPathConfiguration.PathExtractionFolder = PathExtractionFolder;
            AddUpdateAppSettings("PathExtractionFolder", StaticPathConfiguration.PathExtractionFolder);

            /// Путь к папке с шаблонными файлами
            if (!Directory.Exists(PathTemplatedFolder))
                Directory.CreateDirectory(PathTemplatedFolder);
            StaticPathConfiguration.PathTemplatedFolder = PathTemplatedFolder;
            AddUpdateAppSettings("PathTemplatedFolder", StaticPathConfiguration.PathTemplatedFolder);

            /// Путь к папке с подписанными файлами
            if (!Directory.Exists(PathSignedFolder))
                Directory.CreateDirectory(PathSignedFolder);
            StaticPathConfiguration.PathSignedFolder = PathSignedFolder;
            AddUpdateAppSettings("PathSignedFolder", StaticPathConfiguration.PathSignedFolder);

            /// Путь к папке с резервными файлами
            if (!Directory.Exists(PathBackupFolder))
                Directory.CreateDirectory(PathBackupFolder);
            StaticPathConfiguration.PathBackupFolder = PathBackupFolder;
            AddUpdateAppSettings("PathBackupFolder", StaticPathConfiguration.PathBackupFolder);

            /// Путь к папке с принимаемыми файлами от ФТС
            if (!Directory.Exists(PathReplyFTS))
                Directory.CreateDirectory(PathReplyFTS);
            StaticPathConfiguration.PathReplyFTS = PathReplyFTS;
            AddUpdateAppSettings("PathReplyFTS", StaticPathConfiguration.PathReplyFTS);

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
                if (appSettings == null)
                    Console.WriteLine("Настройки приложения пусты. Необходимо инициализировать базовые настройки.");
                else
                {
                    Console.WriteLine("Setting:\n{");
                    foreach (var key in appSettings.AllKeys)
                        Console.WriteLine($"   {key} => {appSettings[key]}");
                    Console.WriteLine("}");
                }
            }
            catch (ConfigurationErrorsException ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return;
            }
        }

        /// <summary>Чтение и возвращение значения </summary>
        /// <param name="key">Ключ настройки</param>
        /// <returns>Возвращает значение по принимаемому ключу</returns>
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
            get { return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile; }
        }
    }
}