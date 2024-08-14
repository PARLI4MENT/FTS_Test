#define TEST

using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using XMLSigner.OutClass;

namespace XMLSigner
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            string pathToXml = @"Resource\test.xml";

            new NormalizationXml(pathToXml);

            /// VB
            /*
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(new StringReader(File.ReadAllText(pathToXml)));
                XmlElement xmlRoot = xmlDoc.DocumentElement;

                var lastObject = (XmlElement)xmlRoot.GetElementsByTagName("Object", "*")[2];
                NormXML(lastObject);
                Console.WriteLine();
                AtribOrderChanger(lastObject);
                Console.WriteLine();
            */

            Console.Write("\nPress any key...");
            Console.ReadKey();
        }

        /// <summary>
        /// Create XML Tree
        /// </summary>
        /// <param name="element"></param>
        public static void GetTree(XmlElement element)
        {
            if (element.GetType().Equals(typeof(System.Xml.XmlElement)))
            {
                foreach (var node in element.ChildNodes)
                {
                    if (node.GetType().Equals(typeof(System.Xml.XmlElement)))
                    {
                        var elem = (XmlElement)node;
                        Console.WriteLine($"\t{elem.Name}");
                        if (elem.InnerText == "" && elem.InnerText == null)
                            GetTree(elem);
                    }
                }
            }
        }

        /// <summary> ДОДЕЛАТЬ </summary>
        /// <param name="rootDoc"></param>
        /// <returns></returns>
        public static void SearchElementForHash(XmlElement rootDoc)
        {
            string prefix = "n1";

            foreach (var xmlElem in rootDoc)
            {
                if (xmlElem.GetType().Equals(typeof(XmlElement)))
                {
                    var xmlCurrentElem = (XmlElement)xmlElem;

                    if (xmlCurrentElem.Name == "Reference")
                    {
                        string tmp = GetPathXMLElement(xmlCurrentElem);
                        Console.WriteLine(tmp);
                    }
                    SearchElementForHash(xmlCurrentElem);
                }
            }
        }

        /// <summary></summary>
        /// <param name="xmlElement"></param>
        /// <returns></returns>
        private static string GetXpathXMLIter(this XmlElement xmlElement)
        {
            string path = "/" + xmlElement.Name;
            XmlElement parentElement = xmlElement.ParentNode as XmlElement;

            if (parentElement != null)
            {
                XmlNodeList xmlNodeList = parentElement.SelectNodes(xmlElement.Name);
                if (xmlNodeList != null && xmlNodeList.Count > 1)
                {
                    int position = 1;
                    foreach (XmlElement xmlElementSibling in xmlNodeList)
                    {
                        if (xmlElementSibling == xmlElement)
                            break;

                        position++;
                    }

                    path = path + $"[{position}]";
                }
                path = parentElement.GetXpathXMLIter() + path;
            }

            return path;
        }

        /// <summary></summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string GetPathXMLElement(XmlNode node)
        {
            string path = node.Name;
            XmlNode search = null;
            while ((search = node.ParentNode).NodeType != XmlNodeType.Document)
            {
                path = search.Name + "/" + path; // Add to path
                node = search;
            }
            return "//" + path;
        }

        public static void NormXML(XmlElement nodeXml)
        {
            XmlAttributeCollection all_atr = nodeXml.Attributes;
            var AtrName = new string[all_atr.Count, 2];
            int i = 0;

            foreach (XmlAttribute atr in all_atr)
            {
                if (atr.Name == "Id" || atr.Name == "DocumentModeID" || atr.Name == "Algorithm"
                    || atr.Name == "URI" || atr.Name == "cols")
                {
                    AtrName[i, 0] = atr.Name;
                    AtrName[i, 1] = atr.Value;
                    i += 1;
                }
            }

            nodeXml.RemoveAllAttributes();
            nodeXml.Prefix = "n1";
            nodeXml.IsEmpty = false;

            for (int a = 0, loopTo = i - 1; a <= loopTo; a++)
            {
                nodeXml.SetAttribute(AtrName[a, 0], AtrName[a, 1]);
            }

            foreach (XmlNode node in nodeXml.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    NormXML((XmlElement)node);
                }
            }
        }

        public static void AtribOrderChanger(XmlElement nodexml)
        {
            string a, b, c, d;

            if (nodexml.Attributes.Count == 2)
            {
                if (nodexml.Attributes[0].Name != "xmlns")
                {
                    a = nodexml.Attributes[0].Name;
                    b = nodexml.Attributes[0].Value;
                    c = nodexml.Attributes[1].Name;
                    d = nodexml.Attributes[1].Value;
                    nodexml.RemoveAllAttributes();
                    nodexml.SetAttribute(c, d);
                    nodexml.SetAttribute(a, b);
                }
            }

            foreach (XmlNode node in nodexml.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    AtribOrderChanger((XmlElement)node);
                }
            }
        }
    }
}
