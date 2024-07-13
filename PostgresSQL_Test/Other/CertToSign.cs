#define Test

using System.Net.WebSockets;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using GostCryptography.Base;
using GostCryptography.Xml;
using GostCryptography.Config;
using CryptoPro.Security.Cryptography;

namespace SqlTest
{
    public class CertToSign
    {
        private static X509Certificate2 _cert;
        public static X509Certificate2 SelectedCertificate { get { return _cert; } private set { _cert = SelectSerificate(); } }
        
        public static X509Certificate2 SelectSerificate()
        {
            var store = new X509Store("MY", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var collection = store.Certificates;
            var certTmp = store.Certificates[1];
            Console.WriteLine(certTmp.GetKeyAlgorithmParametersString());
            Console.WriteLine(certTmp.GetKeyAlgorithm());

            //var certTmp = X509Certificate2UI.SelectFromCollection(collection, "Select", "Select sertificate to sign", X509SelectionFlag.SingleSelection);

            store.Close();

            //#if DEBUG
            //            {
            //                Debug.WriteLine($"IssuerName => {certTmp[0].IssuerName.ToString()}");
            //                Debug.WriteLine($"IssuerName.Name => {certTmp[0].IssuerName.Name}");
            //                Debug.WriteLine($"IssuerName.Oid => {certTmp[0].IssuerName.Oid.ToString()}");
            //                Debug.WriteLine($"IssuerName.RawData => {certTmp[0].IssuerName.RawData.ToString()}");
            //                Debug.WriteLine($"Subject => {certTmp[0].Subject}");
            //                Debug.WriteLine($"SubjectName.Name => {certTmp[0].SubjectName.Name}");
            //                Debug.WriteLine($"SubjectName.Oid => {certTmp[0].SubjectName.Oid.ToString()}");
            //                Debug.WriteLine($"Version => {certTmp[0].Version.ToString()}");
            //                Debug.WriteLine("");
            //            }
            //#endif

            return certTmp;
        }

    }
}
