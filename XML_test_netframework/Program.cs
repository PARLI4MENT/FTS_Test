using System;
using System.IO;
using System.Xml;

namespace XML_test_netframework
{
    class Program
    {
        static void Main(string[] args)
        {
                string path = "C:\\_test\\_test\\TEST.xml";

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(new StringReader(File.ReadAllText(path)));

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(File.ReadAllText(path))))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Reference")
                    {
                        Console.WriteLine(xmlReader.ReadElementContentAsString());
                    }
                }
            }

            //XmlElement xRoot = xmlDocument.DocumentElement;
            //if (xRoot != null )
            //{
            //    foreach (XmlElement xNode in xRoot)
            //    {
            //        if (xNode.Name.ToString() == "Body")
            //        {
            //            if (xNode.Attributes.GetNamedItem("URI") != null)
            //                Console.WriteLine(xNode.Attributes.GetNamedItem("URI").Value.ToString());
            //        }
            //    }
            //    Console.WriteLine();
            //}

            Console.ReadKey();
        }
    }
}
