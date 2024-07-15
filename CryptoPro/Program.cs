using GostCryptography.Xml;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace CryptoPro
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string pathXml = @"C:\_test\EncryptedXmlExample.xml";
            var certificate = FindGostCertificate();

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(new StringReader(File.ReadAllText(pathXml)));
            var expectedXml = xmlDocument.OuterXml;

            var encryptedXml = new GostEncryptedXml();

            var elements = xmlDocument.SelectNodes("//SomeElement[@Encrypt='true']");


            if (elements != null)
            {
                foreach (XmlElement element in elements)
                {
                    // Шифрация элемента
                    var elementEncryptedData = encryptedXml.Encrypt(element, certificate);

                    // Замена элемента его зашифрованным представлением
                    GostEncryptedXml.ReplaceElement(element, elementEncryptedData, false);
                }
            }


            Console.ReadKey();
        }

        public static X509Certificate2 FindGostCertificate(Predicate<X509Certificate2> filter = null)
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
    }
}
