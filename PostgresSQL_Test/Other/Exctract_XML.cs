using Microsoft.IdentityModel.Tokens;
using System.Xml;

namespace PostgresSQL_Test.Other
{
    class ExtractXML
    {
        public static string ExtractId(string pathToXML)
        {
            using (StreamReader sr = new StreamReader(pathToXML, true))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(sr);

                foreach (XmlNode xmlNode in xmlDoc.DocumentElement)
                {
                    if (xmlNode.Name == "DocumentID")
                    {
#if DEBUG
                        Console.WriteLine($"Node Name => {xmlNode.Name}");
                        Console.WriteLine($"Node InnerText => {xmlNode.InnerText}");
                        Console.WriteLine();
#endif
                        return xmlNode.InnerText;
                    }
                }

                return string.Empty;
            }
        }

        public static void ExtractId(string pathToXML, out string id, out string fileName)
        {
            if (pathToXML.IsNullOrEmpty())
            {
                Console.WriteLine("IS NULL OR EMPTY");
                id = null;
                fileName = null;
                return;
            }

            using (StreamReader sr = new StreamReader(pathToXML, true))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(sr);

                foreach (XmlNode xmlNode in xmlDoc.DocumentElement)
                {
                    if (xmlNode.Name == "DocumentID")
                    {
#if DEBUG
                        Console.WriteLine($"Node Name => {xmlNode.Name}");
                        Console.WriteLine($"Node InnerText => {xmlNode.InnerText}");
                        Console.WriteLine();
#endif
                        id = xmlNode.InnerText;
                        fileName = Path.GetFileName(pathToXML);
                        return;
                    }
                }
                id = null;
                fileName= null;

            }
        }
    }
}
