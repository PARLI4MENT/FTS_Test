using GostCryptography.Base;
using GostCryptography.Gost_R3411;
using GostCryptography.Pkcs;
using GostCryptography.Xml;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XMLSigner
{
    public class SignXMLGost
    {
        public static X509Certificate2 Certificate = FindGostCertificateCurrent();

        /// <summary>
        /// Вычисление Hash строки, подписание
        /// </summary>
        /// <param name="certificate"></param>
        public static void SignedXml(X509Certificate2 certificate)
        {
            string pathToXml = @"C:\_test\_test\test.xml";

            {
                //XDocument xDocMain = XDocument.Load(new StringReader(File.ReadAllText(pathToXml)), LoadOptions.SetBaseUri);
                //XPathNavigator xNavi = xDocMain.CreateNavigator();
                //string strExp = "/Envelope/Body/Signature/Object/ArchAddDocRequest/ArchDoc/Signature/Object/Envelope/Body/Signature/Object/ArchAddDocRequest/ArchDoc/Signature/KeyInfo";
                //XPathNodeIterator xPathNodeIter = xNavi.Select(strExp);

            }

            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(new StringReader(File.ReadAllText(pathToXml)));
                var xmlRootNode = xmlDoc.DocumentElement;

                /// [2]Object => [0]ArchAddDocRequest => [4]ArchDoc => [0]Signature => [2]KeyInfo
                //var xmlNodeKeyInfo = xmlRootNode.GetElementsByTagName("Object", "*")[2].ChildNodes[0].ChildNodes[4].ChildNodes[0].ChildNodes[2];
                var xmlNodeKeyInfo = ((XmlElement)xmlRootNode.GetElementsByTagName("Object", "*")[2]).GetElementsByTagName("KeyInfo", "*")[0];

                /// Хеширование KeyInfo
                var xmlNodeRefKeyInfoDigVal = xmlRootNode.GetElementsByTagName("Object", "*")[2].ChildNodes[0].ChildNodes[4].ChildNodes[0].ChildNodes[0].ChildNodes[2].ChildNodes[2];
                xmlNodeRefKeyInfoDigVal.InnerText = HashGostR3411_2012_256(xmlNodeKeyInfo.OuterXml);

                Console.WriteLine(xmlNodeKeyInfo.OuterXml);
                Console.WriteLine();
                Console.WriteLine(HashGostR3411_2012_256(xmlNodeKeyInfo.OuterXml));

                //xmlDoc.Save(pathToXml);

                Console.ReadKey();
            }

            /// Парсинг
            /*
            //{
            //    XmlDocument xmlDoc = new XmlDocument();
            //    xmlDoc.Load(new StringReader(File.ReadAllText(path_ObjectWithHash)));

            //    var xDoc = XDocument.Parse(xmlDoc.OuterXml);

            //}
            */

            /*
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
            */

            Console.WriteLine();
        }

        

        public static string Normalization(string OuterXml)
        {
            string NormalString = string.Empty;
            return NormalString;
        }

        /// <summary> Вычисление хэша по ГОСТ Р 34.11-2012/256 </summary>
        /// <param name="DataForHash"></param>
        /// <returns>Возвращает строку с Hash</returns>
        public static string HashGostR3411_2012_256(string DataForHash)
        {
            var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(DataForHash));

            byte[] hashValue;

            using (var hash = new Gost_R3411_2012_256_HashAlgorithm(ProviderType.CryptoPro))
                hashValue = hash.ComputeHash(dataStream);

            return Convert.ToBase64String(hashValue);
        }

        /// <summary>  Подписание строки </summary>
        /// <param name="message"></param>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public static string SignCmsMessage(string message, X509Certificate2 certificate)
        {
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            // Создание объекта для подписи сообщения
            var signedCms = new GostSignedCms(new ContentInfo(byteMessage));

            // Создание объект с информацией о подписчике
            var signer = new CmsSigner(certificate);

            // Включение информации только о конечном сертификате (только для теста)
            signer.IncludeOption = X509IncludeOption.None;

            // Создание подписи для сообщения CMS/PKCS#7
            signedCms.ComputeSignature(signer);

            // Создание сообщения CMS/PKCS#7
            return Convert.ToBase64String(signedCms.Encode());
        }

        private static bool VerifyMessageCms(string message)
        {
            byte[] signedMessage = Convert.FromBase64String(message);

            var signedCms = new GostSignedCms();

            signedCms.Decode(signedMessage);
            
            try { signedCms.CheckSignature(true); }
            catch (Exception ex) { Console.WriteLine(ex.Message); return false; }

            return true;
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
        public static X509Certificate2 FindGostCertificateCurrent(StoreLocation storeLocation = StoreLocation.CurrentUser)
        {
            var store = new X509Store(StoreName.My, storeLocation);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
            return store.Certificates[1];
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
