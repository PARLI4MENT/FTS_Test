#define TEST

using Npgsql.PostgresTypes;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace XMLSigner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            {
                /// Хэширование
                //string strs = "<n1:KeyInfo xmlns:n1=\"http://www.w3.org/2000/09/xmldsig#\" Id=\"KeyInfo\"><n1:X509Data><n1:X509Certificate></n1:X509Certificate></n1:X509Data></n1:KeyInfo>";
                //Console.WriteLine(SignXMLGost.HashGostR3411_2012_256(strs));

                //Console.WriteLine();
            }

            {

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
                Console.WriteLine("DONE !");


                //// Test to Access DB
                //{
                //    //'Unrecognized database format 'C:\testACCDB.accdb'.' 
                //    //AccessDB.ConnectToAccessWithAce(AccessDB.PathToACCDB);
                //}

            }

            /// XML
            {
                //string pathToXml = @"Resource\test.xml";

                //XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.Load(new StringReader(File.ReadAllText(pathToXml)));
                //var xmlRootNode = xmlDoc.DocumentElement;

                ///// [2]Object => [0]ArchAddDocRequest => [4]ArchDoc => [0]Signature => [2]KeyInfo
                ////var xmlNodeKeyInfo = xmlRootNode.GetElementsByTagName("Object", "*")[2].ChildNodes[0].ChildNodes[4].ChildNodes[0].ChildNodes[2];

                ///// Test Set => <KeyInfo><...></KeyInfo>>
                //var xmlNodeKeyInfo = (XmlElement)xmlRootNode.GetElementsByTagName("Object", "*")[2];



                //Console.WriteLine("Base XmlNode => ");
                //Console.WriteLine(xmlNodeKeyInfo.OuterXml);
                //Console.WriteLine();

                //XmlNode xmlNodeTest = Normalization(xmlNodeKeyInfo, "n1", true);
                //Console.WriteLine(xmlNodeTest.OuterXml);
            }

            /// X XML
            //{
            //    string pathToXml = @"Resource\test.xml";
            //    XDocument xDoc = XDocument.Load(pathToXml);
            //    var xNodeRoot = xDoc.Root;

            //}

            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

        public static XElement Normalization(XElement xElement, string prefix = "n1", bool rootElement = false)
        {


            return null;
        }

        public static XmlNode Normalization(XmlElement xmlNode, string prefix = "n1", bool rootNode = false)
        {
            /// Установка перфикса
            xmlNode.Prefix = prefix;

            if (rootNode)
            {

            }


            ///// Нормализация
            //if (xmlNode.HasChildNodes)
            //{
            //    var childsNodes = xmlNode.ChildNodes;
            //    foreach (XmlNode childNode in childsNodes)
            //    {
            //        Normalization(childNode);
            //    }
            //}


            return xmlNode;
        }

        /// <summary> Смена пространство имён и атрибута местами </summary>
        /// <param name="xmlNode"></param>
        private static void NormalizeSwapAttribute(XmlNode xmlNode)
        {

        }

        private static XmlNode NormalizeAttribute(XmlElement xmlNode, XmlNode xmlNodeParent = null, bool rootNode = false)
        {
            // Если root элемент и кол-во
            if (rootNode && xmlNode.NamespaceURI.Count() > 0)
            {

            }

            /// Удаляем ненужные Namespace из ноды
            if (xmlNode.NamespaceURI != String.Empty)
                NormalizeSwapAttribute(xmlNode);


            return xmlNode;
        }
    }
}
