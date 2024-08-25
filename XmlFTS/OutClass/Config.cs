﻿using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Windows;

namespace XmlFTS.OutClass
{
    public static class Config
    {
        /// <summary>
        /// Инициализация базовой конфигурация. Пути к папкам, шаблонам итд.
        /// Файл с XML шаблоном должен лежать в базовой папке и называться template.xml
        /// Если файл с шаблоном отсутсвует, то вызвается окно выбора файла, затем файл копируется в корневую базовую папку
        /// </summary>Инициализация базовой конфигурация. Пути к папкам, шаблонам итд.
        /// <param name="basePath"> Путь к базовой папке, в которой будут располагаться необходимые папки для работы</param>
        public static void BaseConfiguration([Optional]string basePath)
        {
            if (!string.IsNullOrEmpty(basePath) && Directory.Exists(basePath))
            {
                StaticPathConfiguration.PathRawFolder = Path.Combine(basePath, "rawFiles");
                if (!Directory.Exists(StaticPathConfiguration.PathRawFolder))
                    Directory.CreateDirectory(StaticPathConfiguration.PathRawFolder);

                StaticPathConfiguration.PathIntermidateFolder = Path.Combine(basePath, "intermidateFiles");
                if (!Directory.Exists(StaticPathConfiguration.PathIntermidateFolder))
                    Directory.CreateDirectory(StaticPathConfiguration.PathIntermidateFolder);

                StaticPathConfiguration.PathImplementFolder = Path.Combine(basePath, "implementFiles");
                if (!Directory.Exists(StaticPathConfiguration.PathImplementFolder))
                    Directory.CreateDirectory(StaticPathConfiguration.PathImplementFolder);

                StaticPathConfiguration.PathSignedFolder = Path.Combine(basePath, "signedFiles");
                if (!Directory.Exists(StaticPathConfiguration.PathSignedFolder))
                    Directory.CreateDirectory(StaticPathConfiguration.PathSignedFolder);

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = basePath;
                openFileDialog.Filter = "Файл XML (.xml)|*.xml";
                openFileDialog.FilterIndex = 0;
                openFileDialog.Multiselect = false;
                openFileDialog.RestoreDirectory = true;
                
                var result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    
                }

                return;
            }
            basePath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public static void BaseConfiguration(string PathRawFolder, string PathIntermidateFolder, string PathImplementFolder, string PathSignedFolder, string TemplateXML)
        {

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
            catch (ConfigurationErrorsException ex) { Debug.WriteLine(ex.Message); return; }
        }

        public static string ReadSettings(string key)
        {
            try
            {
                var appSetting = ConfigurationManager.AppSettings;
                string result = appSetting[key] ?? "Поле не найдено";
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