#define TEST

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XMLSigner
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            string pathToXml = @"Resource\test.xml";

            /// XMLDocument
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(new StringReader(File.ReadAllText(pathToXml)));
                XmlElement xmlRoot = xmlDoc.DocumentElement;

                var lastObject = ((XmlElement)xmlRoot.GetElementsByTagName("Object", "*")[2]).GetElementsByTagName("ArchAddDocRequest", "*")[0];
                //var lastObject = (XmlElement)xmlRoot.GetElementsByTagName("Object", "*")[2];

                XmlDocument newXmlDoc = new XmlDocument();
                XmlElement newXmlElem = newXmlDoc.DocumentElement;

                Console.WriteLine(lastObject.Attributes.Count);
                Console.WriteLine(lastObject.OuterXml);

                Normalization(lastObject, newXmlElem);


                Console.WriteLine();
                Console.WriteLine(lastObject.Attributes.Count);
                Console.WriteLine(lastObject.OuterXml);
                Console.WriteLine();
            }

            ///XDocument
            /*
            //    Console.WriteLine();
            //    XDocument xDoc = XDocument.Load(pathToXml);
            //    //IEnumerable<XElement> nodeList = xDoc.Descendants().Where(e => e.Name.LocalName == "Object");
            //    var tmp = xDoc.Element("Object")
            //        .Elements("ArchAddDocRequest");

            //    Console.WriteLine();
            */            

            //Console.WriteLine(SignXMLGost.HashGostR3411_2012_256(lastObject.OuterXml));
            //Console.WriteLine();
            /*
                /// Хэширование
                //string strs = "<n1:KeyInfo xmlns:n1=\"http://www.w3.org/2000/09/xmldsig#\" Id=\"KeyInfo\"><n1:X509Data><n1:X509Certificate></n1:X509Certificate></n1:X509Data></n1:KeyInfo>";
                //Console.WriteLine(SignXMLGost.HashGostR3411_2012_256(strs));

                //Console.WriteLine();1z
            */

            /*
            //XmlNs.ImplementateToXml.ImplementLinear("C:\\_test\\rawFiles\\0be68d4a-444d-4abb-a09f-ce07c9256e30\\files\\05fcc4ca-cfc1-4b59-a67c-d9a1c909b4cb\\xml\\1f2aa4ac-e439-45f6-b4ce-0a21b4f9fcb9.FreeBinaryDoc.xml");
            //SignXMLGost.SignedCmsXml(SignXMLGost.Certificate);

            /// Rename and move to intermidateFiles XML files
            //FileNs.RenamerXML.RenameMoveParallel("C:\\_test\\rawFiles");

            //AccessDB.PathToMDB = "C:\\_test\\testMDB.mdb";

            //var sw = new Stopwatch();
            //var swTotal = new Stopwatch();
            //sw.Start();
            //swTotal.Start();

            //Inplement to XML, signing and sending request to BD
            //Console.WriteLine("\nStart implement...");
            //XmlNs.ImplementateToXml.ImplementParallel(Directory.GetFiles("C:\\_test\\intermidateFiles"));
            //sw.Stop();
            //Console.WriteLine($"Time implement => {sw.ElapsedMilliseconds / 1000},{sw.ElapsedMilliseconds % 1000} sec");
            //Console.WriteLine($"Total destination files => {Directory.GetFiles("C:\\_test\\implementFiles").Count()} units");
            //Console.WriteLine($"AVG => {Directory.GetFiles("C:\\_test\\intermidateFiles").Count() / ((int)sw.ElapsedMilliseconds / 1000)}");

            //sw.Restart();
            //Console.WriteLine("\nStart signing XML...");

            //SignXMLGost.SignFullXml(Directory.GetFiles("C:\\_test\\implementFiles"), SignXMLGost.Certificate);

            //sw.Stop();
            //swTotal.Stop();
            //Console.WriteLine($"Time signed => {sw.ElapsedMilliseconds / 1000},{sw.ElapsedMilliseconds % 1000} sec");
            //Console.WriteLine($"Total destination files => {Directory.GetFiles("C:\\_test\\signedFiles").Count()} units");
            //Console.WriteLine($"AVG => {(Directory.GetFiles("C:\\_test\\implementFiles").Count()) / ((int)sw.ElapsedMilliseconds / 1000)}");

            //Console.WriteLine($"\nTotal time => {swTotal.ElapsedMilliseconds / 1000},{swTotal.ElapsedMilliseconds % 1000} sec");

            //// Test to Access DB
            //{
            //    //'Unrecognized database format 'C:\testACCDB.accdb'.' 
            //    //AccessDB.ConnectToAccessWithAce(AccessDB.PathToACCDB);
            //}
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

        public static string NormalizationX()
        {


            return null;
        }

        /// <summary>Нормализация XML элемента</summary>
        /// <param name="xmlElement"></param>
        /// <param name="prefix"></param>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static void Normalization(XmlNode xmlNode, XmlNode newXmlNode, string prefix = "n1", bool rootNode = false)
        {
            if (xmlNode.GetType().Equals(typeof(XmlElement)))
            {
                var elem = (XmlElement)xmlNode;

                if (elem.HasChildNodes)
                    foreach (var node in elem.ChildNodes)
                        if (node.GetType().Equals(typeof(XmlElement)))
                            Normalization((XmlElement)node, newXmlNode);

                if (!elem.HasChildNodes && elem.InnerText == "")
                    elem.InnerText = "";
                
                elem.Prefix = prefix;

                if (elem.HasAttributes)
                {
                    string attNsName;
                    string attNsValue;

                    for (int i = 0; i < elem.Attributes.Count;)
                    {
                        if (elem.Attributes[i].Name.Contains("xmlns") && !elem.Attributes[i].Value.Contains(elem.NamespaceURI))
                        {
                            
                            elem.RemoveAttributeAt(i);
                            continue;
                        }
                        i++;
                    }

                    /*
                    ///Swap Namespace and Attribute
                    //    Console.WriteLine();

                    //    if (elem.LocalName == "ArchAddDocRequest")
                    //    {
                    //        Console.WriteLine();
                    //        //XDocument xDoc = XDocument.Load(new XmlNodeReader((XmlNode)elem));
                    //        //XElement xElem = XElement.Parse(elem.OuterXml);

                    //        Console.WriteLine();
                    //    }

                    //    {
                    //        XmlSerializerNamespaces xmlSerNs = new XmlSerializerNamespaces();
                    //        xmlSerNs.Add("", "");
                    //    }

                    //    {
                    //        XmlSerializer s = new XmlSerializer(typeof(XmlElement));
                    //        StringWriter sw = new StringWriter();
                    //        s.Serialize(sw, elem);
                    //    }
                    */

                }
            }
        }
    }
}
