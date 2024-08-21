using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace XmlFTS.OutClass
{
    public static class StaticPathConfiguration
    {
        /// <summary> Не доделанно </summary>
        public static void InitializePath()
        {
            Type type = typeof(StaticPathConfiguration);

            /// Get add static memebers of This class
            FieldInfo[] staticFiled = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            /// Layout the names of all static member
            Dictionary<string, string> map = new Dictionary<string, string>();
            foreach (FieldInfo field in staticFiled)
            {
                object value = field.GetValue(null);
                map.Add(field.Name, value.ToString());
            }
            foreach (string key in map.Keys)
                Console.WriteLine("{0}={1}", key, map[key].ToString());
        }

        /// <summary> Путь к XML шаблону </summary>
        public static string TemplateXML { get; set; } = "C:\\_test\\create_doc_in_arch.xml";

        /// <summary> Путь к папке с иходными файлами </summary>
        public static string PathRawFolder { get; set; } = "C:\\_test\\rawFiles";

        /// <summary> Path to Intermidate folder </summary>
        public static string PathIntermidateFolder { get; set; } = "C:\\_test\\intermidateFiles";

        /// <summary> Path to Implement folder</summary>
        public static string PathImplementFolder { get; set; } = "C:\\_test\\implementFiles";

        /// <summary> Path to Signed folder </summary>
        public static string PathSignedFolder { get; set; } = "C:\\_test\\signedFiles";
    }
}
