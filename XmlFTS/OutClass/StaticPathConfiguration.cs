using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;

namespace XmlFTS.OutClass
{
    public static class StaticPathConfiguration
    {
        /// <summary> Не доделанно System.Reflection </summary>
        private static void InitializePath()
        {
            Type type = typeof(StaticPathConfiguration);

            /// Добавление статических членов текущего класса
            FieldInfo[] staticFiled = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            /// Вывод имён всех статических членов класса
            Dictionary<string, string> map = new Dictionary<string, string>();

            foreach (FieldInfo field in staticFiled)
            {
                object value = field.GetValue(null);
                map.Add(field.Name, value.ToString());
            }

            foreach (string key in map.Keys)
                Debug.WriteLine("{0}={1}", key, map[key].ToString());
        }

        public static void BaseInitialize() { }

        /// <summary> Путь к XML шаблону </summary>
        public static string TemplateXML { get; set; }

        /// <summary> Путь к папке с "сырыми" файлами (RawFolder) </summary>
        public static string PathRawFolder { get; set; }

        /// <summary> Путь к папке с извлечёнными файлами (ExtractionFolder) </summary>
        public static string PathExtractionFolder { get; set; }

        /// <summary> Путь к папке с шаблонными файлами (ImplementFolder) </summary>
        public static string PathTemplatedFolder { get; set; }

        /// <summary> Путь к папке с подписанными файлами (SignedFolder) </summary>
        public static string PathSignedFolder { get; set; }

        /// <summary> Путь к папке с резервными копиями файлов (BackupFolder) </summary>
        public static string PathBackupFolder { get; set; }
    }
}