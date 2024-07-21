#define TEST

using GostCryptography.Base;
using GostCryptography.Xml;

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Linq;
using Npgsql;
using SQLTestNs;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Dynamic;

namespace XMLSigner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Setting cert to variable
            var certificate = FindGostCertificate();

            Console.WriteLine("Start process...");

            // Rename and move to intermidateFiles XML files
            FileNs.RenamerXML.RenameMoveParallel("C:\\_test\\rawFiles");

            var sw = new Stopwatch();
            var swTotal = new Stopwatch();
            sw.Start();
            swTotal.Start();

            // Inplement to XML, signing and sending request to BD
            Console.WriteLine("Start implement...");
            XmlNs.ImplementateToXml.ImplementParallel(Directory.GetFiles("C:\\_test\\intermidateFiles"));
            sw.Stop();

            Console.WriteLine($"Time inplement => {sw.ElapsedMilliseconds / 1000},{sw.ElapsedMilliseconds % 1000} sec");
            Console.WriteLine($"Total destination files => {Directory.GetFiles("C:\\_test\\implementFiles").Count()} units");
            Console.WriteLine($"AVG => {(Directory.GetFiles("C:\\_test\\intermidateFiles").Count()) / ((int)sw.ElapsedMilliseconds / 1000)}");

            sw.Restart();
            Console.WriteLine("\nStart signing XML...");
            string[] singingFiles = Directory.GetFiles("C:\\_test\\implementFiles");
            Parallel.ForEach(singingFiles,
                new ParallelOptions { MaxDegreeOfParallelism = -1 },
                singFile =>
                {
                    var signedXml = SignXmlDocument(singFile, ref certificate);
                    signedXml.Save(Path.Combine("C:\\_test\\signedFiles", ("Signed." + Path.GetFileName(singFile))));
                });

            //sw.Stop();
            //swTotal.Stop();
            //Console.WriteLine($"Time signed => {sw.ElapsedMilliseconds/1000},{sw.ElapsedMilliseconds%1000} sec");
            //Console.WriteLine($"Total destination files => {Directory.GetFiles("C:\\_test\\signedFiles").Count()} units");
            //Console.WriteLine($"AVG => {(Directory.GetFiles("C:\\_test\\implementFiles").Count())/((int)sw.ElapsedMilliseconds/1000)}");


            //Console.WriteLine($"\nTotal time => {swTotal.ElapsedMilliseconds/1000},{swTotal.ElapsedMilliseconds%1000} sec");
            //Console.WriteLine("DONE !");
            //Console.ReadKey();
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
