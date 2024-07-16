using GostCryptography.Base;
using GostCryptography.Xml;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace CryptoPro
{
    internal class Program
    {
        static void Main(string[] args)
        {
            


            /// Rename file from inputFiles to outputFiles
            {
                Console.WriteLine("Start process...");
                SqlTest.RenamerXML renamerXML = new SqlTest.RenamerXML();
                renamerXML.RenameAndMoveParallel();
            }

            string pathXml = @"C:\_test\0be68d4a-444d-4abb-a09f-ce07c9256e30.1f2aa4ac-e439-45f6-b4ce-0a21b4f9fcb9.FreeBinaryDoc.xml";
            var certificate = FindGostCertificate();

            /// Remove SignedFiles from singedFiles folder
            {
                string[] signedFiles = Directory.GetFiles("");
                if (signedFiles.Count() != 0)
                    foreach (var signedFile in signedFiles)
                        File.Delete(signedFile);
            }

            string[] outputFile = Directory.GetFiles("C:\\_test\\inputFiles");
            foreach (string path in outputFile)
            {
                var signedXml = SignXmlDocument(path, ref certificate);
                signedXml.Save(Path.Combine("C:\\_test\\signedFiles", ("Signed_" + Path.GetFileName(pathXml))));
            }


            Console.ReadKey();
        }

        /// <summary>
        /// Подписание XML-документа
        /// </summary>
        /// <param name="pathToXmlFile">Абсолютный путь до XML-файла</param>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public static XmlDocument SignXmlDocument(string pathToXmlFile, ref X509Certificate2 certificate)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(new StringReader(File.ReadAllText(pathToXmlFile)));

            var signedXml = new GostSignedXml(xmlDocument);
            signedXml.SetSigningCertificate(certificate);
            
            var dataReference = new Reference { Uri = "", DigestMethod = GetDigestMethod(certificate) };
            dataReference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            

            signedXml.AddReference(dataReference); ;

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificate));
            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();

            var signatureXml = signedXml.GetXml();

            xmlDocument.DocumentElement.AppendChild(xmlDocument.ImportNode(signatureXml, true));
            return xmlDocument;
        }

        /// <summary>
        /// Получение объекта сертификата из хранилища
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static X509Certificate2 FindGostCertificate(Predicate<X509Certificate2> filter = null)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

            try
            {
                foreach (var certificate in store.Certificates)
                {
                    if (certificate.HasPrivateKey && certificate.IsGost() && (filter == null || filter(certificate)))
                    {
                        return certificate;
                    }
                }
            }
            finally
            {
                store.Close();
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        private static string GetDigestMethod(X509Certificate2 certificate)
        {
            using(var publicKey = (GostAsymmetricAlgorithm)certificate.GetPrivateKeyAlgorithm())
            {
                using (var hasAlgorithm = publicKey.CreateHashAlgorithm())
                {
                    return hasAlgorithm.AlgorithmName;
                }
            }
        }
    }
}
