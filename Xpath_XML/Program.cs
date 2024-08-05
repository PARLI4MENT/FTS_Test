using EnvelopXMLSerialize;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace std
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToXml = @"C:\\_test\\_test\\TEST.xml";

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(pathToXml);

            //XmlSerializer serializer = new XmlSerializer(typeof(Envelope));

            //using (XmlReader xmlReader = XmlReader.Create(new StringReader(File.ReadAllText(pathToXml))))
            //{
            //    while (xmlReader.Read())
            //    {

            //    }
            //}

            {
                XPathDocument xDoc = new XPathDocument(pathToXml);
                XPathNavigator xNavigator = xDoc.CreateNavigator();
            }

            Console.ReadKey();
        }
    }
}