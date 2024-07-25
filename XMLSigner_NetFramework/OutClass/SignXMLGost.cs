using GostCryptography.Base;
using GostCryptography.Pkcs;
using GostCryptography.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XMLSigner
{
    public class SignXMLGost
    {
        public static X509Certificate2 Certificate = FindGostCertificate();

        public static void SignedCmsXml(X509Certificate2 certificate)
        {
            string pathToXml = @"C:\\_test\\_test\\TEST.xml";
            //XmlElement xRoot = xmlDoc.DocumentElement;

            //XmlNodeList xmlElement = xRoot.SelectNodes("/Header");

            /*
            {
                XmlNode rootList = xmlDoc.DocumentElement.LastChild;

                foreach (XmlNode node1 in rootList)
                {
                    if (node1.Name == "Signature")
                    {
                        foreach (XmlNode node2 in node1.ChildNodes)
                        {
                            if (node2.Name == "SignedInfo")
                            {
                                foreach (XmlNode node3 in node2)
                                {
                                    if (node3.Name == "Reference")
                                    {
                                        Console.WriteLine(node3.Attributes[0].Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            */

            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(pathToXml);
                XmlNode xmlNodeRoot = xmlDoc.DocumentElement;

                var xmlNodesListObject = (XmlElement)xmlDoc.GetElementsByTagName("Object", "*")[2];
                Console.WriteLine(xmlNodesListObject.OuterXml);
                Console.WriteLine(SignMessage(Certificate, Encoding.UTF8.GetBytes(xmlNodesListObject.OuterXml)));
            }

            Console.WriteLine();
        }

        public static byte[] SignMessage(X509Certificate2 certificate, byte[] message)
        {
            // Создание объекта для подписи сообщения
            var signedCms = new GostSignedCms(new ContentInfo(message));

            // Создание объект с информацией о подписчике
            var signer = new CmsSigner(certificate);

            // Включение информации только о конечном сертификате (только для теста)
            signer.IncludeOption = X509IncludeOption.EndCertOnly;

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
