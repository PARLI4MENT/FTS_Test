using System.Xml;
using System.Security.Cryptography.X509Certificates;

using GostCryptography.Base;
using GostCryptography.Xml;

namespace CryptoPro_Test_Net5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cert = SelectSerificate();

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(new StringReader(File.ReadAllText("C:\\_test\\test.xml")));

            var signedXml = new GostSignedXml(xmlDoc);
            signedXml.SetSigningCertificate(cert);

            Console.ReadKey();
        }
        public static X509Certificate2 SelectSerificate()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);

            var collection = store.Certificates;
            var certTmp = store.Certificates[0];

            store.Close();

            return certTmp;
        }
    }

}
