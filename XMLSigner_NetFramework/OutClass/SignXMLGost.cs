using GostCryptography.Base;
using GostCryptography.Config;
using GostCryptography.Gost_R3411;
using GostCryptography.Pkcs;
using GostCryptography.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Resolvers;
using System.Xml.XPath;
using static System.Net.Mime.MediaTypeNames;

namespace XMLSigner
{
    public class SignXMLGost
    {
        public static X509Certificate2 Certificate = FindGostCertificate();

        public static void SignedCmsXml(X509Certificate2 certificate)
        {
            string pathToXml = @"C:\\_test\\_test\\TEST.xml";
            
            {
            //    {
            //        var tmp = Convert.ToBase64String(SignCmsMessage(Certificate, Encoding.UTF8.GetBytes("<DigestMethod Algorithm=\"urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34112012-256\" />")));
            //        Console.WriteLine(tmp);

            //        Console.WriteLine();

            //        var tmp2 = Convert.ToBase64String(SignCmsMessage(Certificate, Encoding.UTF8.GetBytes("urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34112012-256")));
            //        Console.WriteLine(tmp2);
            //    }

            //    XmlDocument xmlDoc = new XmlDocument();
            //    xmlDoc.Load(new StringReader(File.ReadAllText("C:\\_test\\_test\\TEST.xml")));

            //    var xmlNodesListObject = xmlDoc.GetElementsByTagName("Object", "*")[2];

                
            //    var tmpBase64 = Convert.ToBase64String(SignCmsMessage(Certificate, Encoding.UTF8.GetBytes(xmlNodesListObject.OuterXml)));
            //    Console.WriteLine(tmpBase64);
            }

            // Ручное создание Элементов
            {
                //    /// Base xml 
                //    // Потом заменить на XmlElement из другого базового XML файла
                //    string pathBaseXml = "C:\\_test\\_test\\_Object_Non_hash.xml";
                //    string pathDesXml = $@"Dest_{Path.GetFileName(pathBaseXml)}";

                //    XmlDocument xmlDocBase = new XmlDocument();
                //    xmlDocBase.Load(new StringReader(File.ReadAllText(pathBaseXml)));

                //    var dataObject =
                //        new XElement("Object",
                //        new XAttribute("Id", "Object"),
                //        new XAttribute("", ""),
                //        new XElement("ArchAddDocRequest",
                //        new XAttribute("xmlns", (XNamespace)"urn:customs.ru:Information:EArchDocuments:ArchAddDocRequest:5.13.1"),
                //        new XAttribute("{xmlns:}ct", "urn:customs.ru:Information:EArchDocuments:EADCommonTypes:5.13.1"),
                //        new XAttribute("{xmlns:}xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                //        new XAttribute("DocumentModeID", "1005001E"),
                //        new XElement("{ct:}DocumentID", "6BCC7C9D-9BAE-4FEA-B199-DB2513B0DC05"),
                //        new XElement("{ct:}ArchDeclID", XElement.EmptySequence)

                //        ));

                //    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TEST.xml");
                //    XDocument xmlDocument = new XDocument();
                //    xmlDocument.Add(dataObject);
                //    xmlDocument.Save(path);
            }

            /// Парсинг
            /*
            //{
            //    XmlDocument xmlDoc = new XmlDocument();
            //    xmlDoc.Load(new StringReader(File.ReadAllText(path_ObjectWithHash)));

            //    var xDoc = XDocument.Parse(xmlDoc.OuterXml);

            //}
            */

            //{
            //    //string pathDest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Int.xml");

            //    XmlDocument xmlDoc = new XmlDocument();
            //    xmlDoc.Load(pathToXml);

            //    XmlNode xmlNodeRoot = xmlDoc.DocumentElement;
            //    var xmlNodesListObject = (XmlNode)xmlDoc.GetElementsByTagName("Object", "*")[3];

            //    //(XmlNode)xmlDoc.GetElementsByTagName("Object", "*")[2]
            //    var xmlNodeTemp = xmlDoc.GetElementsByTagName("Object", "*")[2];
            //    xmlNodeTemp.SelectSingleNode("//Object");


            //    Console.WriteLine(HashGostR3411_2012_256(xmlNodeTemp.OuterXml));

            //        Console.WriteLine();
            //}

            Console.WriteLine();
        }

        /// <summary>
        /// Вычисление хэша ГОСТ Р 34.11-2012/256
        /// </summary>
        /// <param name="DataForHash"></param>

        /// <summary>
        /// Вычисление хэша ГОСТ Р 34.11-2012/256
        /// </summary>
        /// <param name="DataForHash"></param>


        public static string HashGostR3411_2012_256(string DataForHash)
        {
            var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(DataForHash));

            byte[] hashValue;

            using (var hash = new Gost_R3411_2012_256_HashAlgorithm(ProviderType.CryptoPro))
                hashValue = hash.ComputeHash(dataStream);

            return Convert.ToBase64String(hashValue);
        }

        public static byte[] SignCmsMessage(X509Certificate2 certificate, byte[] message)
        {
            // Создание объекта для подписи сообщения
            var signedCms = new GostSignedCms(new ContentInfo(message));

            // Создание объект с информацией о подписчике
            var signer = new CmsSigner(certificate);

            // Включение информации только о конечном сертификате (только для теста)
            signer.IncludeOption = X509IncludeOption.None;

            // Создание подписи для сообщения CMS/PKCS#7
            signedCms.ComputeSignature(signer);

            // Создание сообщения CMS/PKCS#7
            return signedCms.Encode();
        }

        private static byte[] CreateMessage()
        {
            return Encoding.UTF8.GetBytes("Some message to sign...");
        }

        public static void SignFullXml(string xmlFile, X509Certificate2 certificate, bool deleteSourceFile = false)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(new StringReader(File.ReadAllText(xmlFile)));

            var signedXml = new GostSignedXml(xmlDocument);
            signedXml.SetSigningCertificate(Certificate);

            var dataReference = new Reference { Uri = "", DigestMethod = GetDigestMethod(Certificate) };
            dataReference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

            signedXml.AddReference(dataReference); ;

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(Certificate));
            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();

            var signatureXml = signedXml.GetXml();

            xmlDocument.DocumentElement.AppendChild(xmlDocument.ImportNode(signatureXml, true));

            xmlDocument.Save(Path.Combine("C:\\_test\\signedFiles", ("Signed." + Path.GetFileName(xmlFile))));

            if (deleteSourceFile)
                File.Delete(Path.GetFullPath(xmlFile));
        }

        public static void SignFullXml(string[] arrXmlFiles, X509Certificate2 certificate, bool deleteSourceFile = false)
        {
            Parallel.ForEach(arrXmlFiles,
                new ParallelOptions { MaxDegreeOfParallelism = -1 },
                xmlFile =>
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(new StringReader(File.ReadAllText(xmlFile)));

                    var signedXml = new GostSignedXml(xmlDocument);
                    signedXml.SetSigningCertificate(Certificate);

                    var dataReference = new Reference { Uri = "", DigestMethod = GetDigestMethod(Certificate) };
                    dataReference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

                    signedXml.AddReference(dataReference); ;

                    var keyInfo = new KeyInfo(); 
                    keyInfo.AddClause(new KeyInfoX509Data(Certificate));
                    signedXml.KeyInfo = keyInfo;

                    signedXml.ComputeSignature();

                    var signatureXml = signedXml.GetXml();

                    xmlDocument.DocumentElement.AppendChild(xmlDocument.ImportNode(signatureXml, true));

                    xmlDocument.Save(Path.Combine("C:\\_test\\signedFiles", ("Signed." + Path.GetFileName(xmlFile))));

                    if (deleteSourceFile)
                        File.Delete(Path.GetFullPath(xmlFile));
                });
        }

        /// <summary>
        /// Получение объекта сертификата из хранилища
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static X509Certificate2 FindGostCertificate(StoreLocation storeLocation = StoreLocation.CurrentUser, Predicate<X509Certificate2> filter = null)
        {
            var store = new X509Store(StoreName.My, storeLocation);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

            try
            {
                foreach (var certificate in store.Certificates)
                {
                    if (certificate.HasPrivateKey && certificate.IsGost() && (filter == null || filter(certificate)))
                        return certificate;
                }
            }
            finally { store.Close(); }

            return null;
        }

        /// <summary>
        /// Получение имени алгоритма
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        private static string GetDigestMethod(X509Certificate2 certificate)
        {
            using (var publicKey = (GostAsymmetricAlgorithm)certificate.GetPrivateKeyAlgorithm())
            {
                using (var hasAlgorithm = publicKey.CreateHashAlgorithm())
                {
                    return hasAlgorithm.AlgorithmName;
                }
            }
        }
    }
}
