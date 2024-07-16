#define TEST

using GostCryptography.Base;
using GostCryptography.Xml;
using SqlTest;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            Console.WriteLine("Start process...");
            SqlTest.RenamerXML renamerXML = new SqlTest.RenamerXML();
            renamerXML.RenameAndMoveParallel();
            //RenamerXML.GetLastStatistics();

            /// Get Certificate
            var certificate = FindGostCertificate();



            /// Clear SignedFiles from singedFiles folder
            var signedFiles = Directory.GetFiles("C:\\_test\\signedFiles");
            if (signedFiles.Count() != 0)
                foreach (var signedFile in signedFiles)
                    File.Delete(signedFile);

            string[] inputFiles = Directory.GetFiles("C:\\_test\\inputFiles");
            Console.WriteLine("Start Signing Xml files from inputFiles folder");
#if TEST
            var swSigning = new Stopwatch();
            swSigning.Start();
#endif
            foreach (string path in inputFiles)
            {
                var signedXml = SignXmlDocument(path, ref certificate);
                signedXml.Save(Path.Combine("C:\\_test\\signedFiles", ("Signed_" + Path.GetFileName(path))));
            }
            swSigning.Stop();
#if TEST
            Console.WriteLine("{");
            Console.WriteLine("\tOperation => ParseXMLByMaskParallel is DONE!");
            Console.WriteLine($"\nTotal time: {swSigning.ElapsedMilliseconds},{swSigning.ElapsedMilliseconds % 1000} msec");
            Console.WriteLine($"\tRaw folder with files: {inputFiles.Count()}");
            Console.WriteLine($"Total files ({Directory.GetFiles("C:\\_test\\inputFiles").Count()} " +
                $"/ {Directory.GetFiles("C:\\_test\\rawFiles").Count()})");
            Console.WriteLine($"AVG time: {Directory.GetFiles("C:\\_test\\inputFiles").Count() / (int)(swSigning.ElapsedMilliseconds / 1000)},"
                 + $"{swSigning.ElapsedMilliseconds % 1000} units");
            Console.WriteLine("}\n");
#endif
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
