using GostCryptography.Asn1.Gost.Gost_R3410_2012_512;
using GostCryptography.Base;
using GostCryptography.Xml;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;

namespace XMLSigner
{
    public class SignXMLGost
    {
        public static X509Certificate2 Certificate = FindGostCertificate();


        /// <summary>
        /// ДОДЕЛАТЬ
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <param name="certificate"></param>
        public static void SignCurrentPartXml(string xmlFile, X509Certificate2 certificate)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(new StringReader(File.ReadAllText(xmlFile)));
            
            var xmlSigned = new GostSignedXml(xmlDoc);
             
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
        private static X509Certificate2 FindGostCertificate(StoreLocation storeLocation = StoreLocation.CurrentUser, Predicate<X509Certificate2> filter = null)
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
