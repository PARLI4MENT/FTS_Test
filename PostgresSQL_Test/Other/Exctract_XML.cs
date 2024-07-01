using System.Xml;

namespace PostgresSQL_Test.Other
{
    class Extract_XML
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
    }
}
