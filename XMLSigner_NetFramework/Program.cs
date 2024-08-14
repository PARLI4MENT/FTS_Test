#define TEST

using GostCryptography.Pkcs;
using GostCryptography.Xml;
using SQLTestNs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XMLSigner
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            string pathToXml = @"Resource\test.xml";

            /// XML Doc Orig
            XmlDocument xmlDocOrigin = new XmlDocument();
            xmlDocOrigin.Load(new StringReader(File.ReadAllText(pathToXml)));
            XmlElement xmlRootOrigin = xmlDocOrigin.DocumentElement;

            /// XML Document Editing
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(new StringReader(File.ReadAllText(pathToXml)));
            XmlElement xmlRoot = xmlDoc.DocumentElement;

            var lastObject = (XmlElement)xmlRoot.GetElementsByTagName("Object", "*")[2];
            var lastObjectKeyInfo = ((XmlElement)xmlRoot.GetElementsByTagName("Object", "*")[2]).GetElementsByTagName("KeyInfo")[0];

            Normalization(lastObjectKeyInfo);
            var finalTextKeyInfo = SwapAttributes(lastObjectKeyInfo.OuterXml);

            Normalization(lastObject);
            var finalTextObject = SwapAttributes(lastObject.OuterXml);

            var tmp2 = SignXMLGost.SignCmsMessage(finalTextObject, SignXMLGost.Certificate);

            Console.WriteLine();
            Console.WriteLine(tmp2);
            Console.WriteLine();

            //Console.WriteLine();
            //Console.WriteLine(SignXMLGost.HashGostR3411_2012_256(finalTextKeyInfo));

            //Console.WriteLine();
            //Console.WriteLine(SignXMLGost.SignCmsMessage(finalTextObject, SignXMLGost.Certificate));


            /// Хэширование
            //Console.WriteLine(SignXMLGost.HashGostR3411_2012_256(strs));

            //Console.WriteLine();

            /*
            XmlNs.ImplementateToXml.ImplementLinear("C:\\_test\\rawFiles\\0be68d4a-444d-4abb-a09f-ce07c9256e30\\files\\05fcc4ca-cfc1-4b59-a67c-d9a1c909b4cb\\xml\\1f2aa4ac-e439-45f6-b4ce-0a21b4f9fcb9.FreeBinaryDoc.xml");
            SignXMLGost.SignedCmsXml(SignXMLGost.Certificate);

            /// Rename and move to intermidateFiles XML files
            FileNs.RenamerXML.RenameMoveParallel("C:\\_test\\rawFiles");

            AccessDB.PathToMDB = "C:\\_test\\testMDB.mdb";

            var sw = new Stopwatch();
            var swTotal = new Stopwatch();
            sw.Start();
            swTotal.Start();

            /// Inplement to XML, signing and sending request to BD
            Console.WriteLine("\nStart implement...");
            XmlNs.ImplementateToXml.ImplementParallel(Directory.GetFiles("C:\\_test\\intermidateFiles"));
            sw.Stop();
            Console.WriteLine($"Time implement => {sw.ElapsedMilliseconds / 1000},{sw.ElapsedMilliseconds % 1000} sec");
            Console.WriteLine($"Total destination files => {Directory.GetFiles("C:\\_test\\implementFiles").Count()} units");
            Console.WriteLine($"AVG => {Directory.GetFiles("C:\\_test\\intermidateFiles").Count() / ((int)sw.ElapsedMilliseconds / 1000)}");

            sw.Restart();
            Console.WriteLine("\nStart signing XML...");

            SignXMLGost.SignFullXml(Directory.GetFiles("C:\\_test\\implementFiles"), SignXMLGost.Certificate);

            sw.Stop();
            swTotal.Stop();
            Console.WriteLine($"Time signed => {sw.ElapsedMilliseconds / 1000},{sw.ElapsedMilliseconds % 1000} sec");
            Console.WriteLine($"Total destination files => {Directory.GetFiles("C:\\_test\\signedFiles").Count()} units");
            Console.WriteLine($"AVG => {(Directory.GetFiles("C:\\_test\\implementFiles").Count()) / ((int)sw.ElapsedMilliseconds / 1000)}");

            Console.WriteLine($"\nTotal time => {swTotal.ElapsedMilliseconds / 1000},{swTotal.ElapsedMilliseconds % 1000} sec");

            // Test to Access DB
            {
                //'Unrecognized database format 'C:\testACCDB.accdb'.' 
                //AccessDB.ConnectToAccessWithAce(AccessDB.PathToACCDB);
            }
            */

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

        /// <summary>ДОДЕЛАТЬ</summary>
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

        /// <summary>Нормализация XML элемента</summary>
        /// <param name="xmlElement"></param>
        /// <param name="prefix"></param>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static void Normalization(XmlNode xmlNode, string prefix = "n1", bool rootNode = false)
        {
            if (xmlNode.GetType().Equals(typeof(XmlElement)))
            {
                var elem = (XmlElement)xmlNode;

                if (elem.HasChildNodes)
                    foreach (var node in elem.ChildNodes)
                        if (node.GetType().Equals(typeof(XmlElement)))
                            Normalization((XmlElement)node);

                elem.Prefix = "n1";
                elem.IsEmpty = false;

                if (elem.HasAttributes)
                {
                    for (int i = 0; i < elem.Attributes.Count;)
                    {
                        if (elem.Attributes[i].Name.Contains("xmlns"))
                        {
                            elem.RemoveAttributeAt(i);
                            continue;
                        }
                        i++;
                    }
                }
            }
        }
        private static string SwapAttributes(string OuterXml, bool disableFormating = true)
        {
            XDocument xDoc = XDocument.Parse(OuterXml);
            XElement xElement = xDoc.Root;

            foreach (var elem in xElement.DescendantNodesAndSelf())
            {
                if (elem is XElement)
                {
                    if (((XElement)elem).HasAttributes && ((XElement)elem).Attributes().Count() > 1)
                    {
                        if (!((XElement)elem).Attributes().First().Name.ToString().Contains("xmlns:n1"))
                        {
                            var fAttr = ((XElement)elem).Attributes().First();
                            var sAttr = ((XElement)elem).Attributes().Last();

                            ((XElement)elem).Attributes().Remove();

                            ((XElement)elem).SetAttributeValue(sAttr.Name, sAttr.Value);
                            ((XElement)elem).SetAttributeValue(fAttr.Name, fAttr.Value);
                        }
                    }
                }
            }

            if (disableFormating)
                return xElement.ToString(SaveOptions.DisableFormatting);
            return xElement.ToString();
        }
    }
}
