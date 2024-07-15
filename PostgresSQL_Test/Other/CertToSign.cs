#define Test

using System.Net.WebSockets;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using GostCryptography.Base;
using GostCryptography.Xml;
using GostCryptography.Config;
using CryptoPro.Security.Cryptography;
using GostCryptography.Gost_R3410;
using Microsoft.IdentityModel.Tokens;
using Windows.Security.Cryptography.Certificates;
using CryptoPro.Security.Cryptography.Xml;

namespace SqlTest
{
    public class CertToSign
    {
        private static X509Certificate2 _cert;
        public static X509Certificate2 SelectedCertificate { get { return _cert; } private set { _cert = SelectSerificate(); } }
        
        public static X509Certificate2 SelectSerificate()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var collection = store.Certificates;
            var cert = store.Certificates[1];



            //GostCryptoConfig.ProviderType = (GostCryptography.Base.ProviderType)certTmp.GetPrivateKeyInfo().ProviderType;
            Console.WriteLine($"{GostCryptoConfig.ProviderType.ToString()}");

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

            return cert;
        }

    }
}
