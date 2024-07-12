#define Test

using System.Net.WebSockets;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


using GostCryptography.Base;
using GostCryptography.Xml;

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

            //var certTmp = X509Certificate2UI.SelectFromCollection(collection, "Select", "Select sertificate to sign", X509SelectionFlag.SingleSelection);

            //Console.WriteLine();

            ////#if DEBUG
            //foreach (X509Certificate2 cert in collection)
            //{
            //    Console.WriteLine($"IssuerName => {cert.IssuerName.ToString()}");
            //    Console.WriteLine($"IssuerName.Name => {cert.IssuerName.Name}");
            //    Console.WriteLine($"IssuerName.Oid => {cert.IssuerName.Oid.ToString()}");
            //    Console.WriteLine($"IssuerName.RawData => {cert.IssuerName.RawData.ToString()}");
            //    Console.WriteLine($"Subject => {cert.Subject}");
            //    Console.WriteLine($"SubjectName.Name => {cert.SubjectName.Name}");
            //    Console.WriteLine($"SubjectName.Oid => {cert.SubjectName.Oid.ToString()}");
            //    Console.WriteLine($"Version => {cert.Version.ToString()}");
            //    Console.WriteLine($"GetPrivateKeyAlgorithm => {((GostAsymmetricAlgorithm)cert.GetPrivateKeyAlgorithm()).ToString}");
            //    Console.WriteLine("");
            //}
            //#endif
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
