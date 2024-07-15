using GostCryptography.Config;
using GostCryptography.Gost_R3410;
using System;
using System.Security.Cryptography.X509Certificates;

namespace CryptoPro_Test_Net5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    public static X509Certificate2 SelectSerificate()
        {
            var store = new X509Store("MY", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var collection = store.Certificates;
            var certTmp = store.Certificates[1];
            GostCryptoConfig.ProviderType = (GostCryptography.Base.ProviderType)certTmp.GetPrivateKeyInfo().ProviderType;
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

            return certTmp;
        }
    }

}
