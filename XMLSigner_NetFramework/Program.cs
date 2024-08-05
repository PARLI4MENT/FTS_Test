#define TEST

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace XMLSigner
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            string pathToXml = @"Resource\test.xml";
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(new StringReader(File.ReadAllText(pathToXml)));
                XmlElement xmlRoot = xmlDoc.DocumentElement;

                var lastObject = xmlRoot.GetElementsByTagName("Object", "*")[2];
                //Console.WriteLine(lastObject.OuterXml);
                Tree((XmlElement)lastObject);

                Console.WriteLine();
                Console.WriteLine(lastObject.OuterXml);

                ///// Get Last "KeyInfo"
                //var lastKeyInfo = ((XmlElement)xmlRoot.GetElementsByTagName("Object", "*")[2]).GetElementsByTagName("KeyInfo", "*")[0];
                //Console.WriteLine(lastKeyInfo.OuterXml);
                //Console.WriteLine();
                //Console.WriteLine();
                //Console.WriteLine(lastKeyInfo.OuterXml);
                //Console.WriteLine();

                ///// Get Last ""
                //Normalization(lastObject);
            }

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

            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

        /// <summary>
        /// Create XML Tree
        /// </summary>
        /// <param name="element"></param>
        public static void Tree(XmlElement element)
        {
            if (element.GetType().Equals(typeof(System.Xml.XmlElement)))
            {
                foreach (var node in element.ChildNodes)
                {
                    if (node.GetType().Equals(typeof(System.Xml.XmlElement)))
                    {
                        var elem = (XmlElement)node;
                        Console.WriteLine($"\t{elem.Name}");
                        if (elem.HasChildNodes)
                        {
                            Tree(elem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ДОДЕЛАТЬ
        /// </summary>
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

        /// <summary> </summary>
        /// <param name="xmlElement"></param>
        /// <param name="prefix"></param>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static void Normalization(XmlNode xmlNode, string prefix = "n1", bool rootNode = false)
        {

            if (xmlNode.HasChildNodes)
            {
                var xmlElems = xmlNode.ChildNodes;
                foreach (XmlNode node in xmlElems)
                {
                    /// Set prefix
                    node.Prefix = prefix;

                    Normalization(node);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(xmlNode.Value))
                {
                    string tmpChildName = xmlNode.Name;
                    XmlNode parentNode = xmlNode.ParentNode;
                    parentNode.RemoveChild(xmlNode);
                    var xmlTextChild = xmlNode.OwnerDocument.CreateElement(tmpChildName);
                    parentNode.AppendChild(xmlTextChild);
                    parentNode.FirstChild.InnerText = "";
                    parentNode.FirstChild.Prefix = prefix;

                    return;
                }
            }

            //if (xmlNode.NamespaceURI.Count() > 0 && (xmlNode.Attributes.Count > 0))
            //    NormalizeAttribute((XmlElement)xmlNode);
        }

        //public static XmlElement Normalization(XmlElement xmlElement, string prefix = "n1", bool rootElement = false)
        //{
        //    xmlElement.Prefix = prefix;

        //    if (xmlElement.HasChildNodes)
        //    {
        //        var xmlElems = xmlElement.ChildNodes;
        //        foreach (XmlElement elem in xmlElems)
        //            Normalization(elem);
        //    }
        //    else
        //    {
        //        if (xmlElement.IsEmpty && string.IsNullOrEmpty(xmlElement.Value))
        //        {

        //            string tmpElemName = xmlElement.Name;
        //            var tmpElem = (XmlElement)xmlElement.ParentNode;
        //            tmpElem.RemoveChild(xmlElement);
        //            tmpElem.AppendChild(tmpElem.OwnerDocument.CreateNode(XmlNodeType.Text, tmpElemName, ""));
        //            var tmpNode = tmpElem.GetElementsByTagName(tmpElemName, "*")[0];
        //            return (XmlElement)tmpNode;
        //        }
        //    }

        //    return xmlElement;
        //}

        /// <summary> Смена пространство имён и атрибута местами </summary>
        /// <param name="xmlElem"></param>
        //private static void NormalizeAttribute(XmlElement xmlElem)
        //{
        //    /// Save Attribute
        //    XmlAttribute[] attrs = new XmlAttribute[] { };
        //    foreach (XmlAttribute attr in xmlElem.Attributes)
        //        attrs = new XmlAttribute[] {  attr };

        //    /// Save Namespace
        //    List<XmlNs> xmlNsList = new List<XmlNs>();
        //    foreach  item in collection)
        //    {

        //    }
        //}

        private struct XmlNs
        {
            public string NamespaceURI { get; set; }
            public string Prefix { get; set; }
        }

        private static XmlNode NormalizeAttribute(XmlElement xmlNode, XmlNode xmlNodeParent = null, bool rootNode = false)
        {
            // Если root элемент и кол-во
            if (rootNode && xmlNode.NamespaceURI.Count() > 0)
            {

            }

            /// Удаляем ненужные Namespace из ноды
            if (xmlNode.NamespaceURI != String.Empty)
                NormalizeAttribute(xmlNode);

            return xmlNode;
        }
    }
}
