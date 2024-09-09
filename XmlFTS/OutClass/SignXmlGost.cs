using GostCryptography.Base;
using GostCryptography.Gost_R3411;
using GostCryptography.Pkcs;
using System;
using System.IO;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace XMLSigner
{
    public class SignXmlGost
    {
        public static X509Certificate2 Certificate;

        /// <summary> Вычисление хэша по ГОСТ Р 34.11-2012/256 </summary>
        /// <param name="DataForHash"></param>
        /// <returns>Возвращает строку с Hash</returns>
        public static string HashGostR3411_2012_256(string DataForHash)
        {
            var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(DataForHash));

            byte[] hashValue;

            using (var hash = new Gost_R3411_2012_256_HashAlgorithm(ProviderType.CryptoPro))
                hashValue = hash.ComputeHash(dataStream);

            return Convert.ToBase64String(hashValue);
        }

        /// <summary>  Подписание строки </summary>
        /// <param name="message"></param>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public static string SignCmsMessage(string message, X509Certificate2 certificate)
        {
            var bytesSrc = Encoding.UTF8.GetBytes(message);

            // Создание объекта для подписи сообщения
            var signedCms = new GostSignedCms(new ContentInfo(bytesSrc), true);

            // Создание объект с информацией о подписчике
            var signer = new CmsSigner(certificate);

            // Включение информации только о конечном сертификате (только для теста)
            signer.IncludeOption = X509IncludeOption.None;

            // Создание подписи для сообщения CMS/PKCS#7
            signedCms.ComputeSignature(signer);

            // Создание подписи CMS/PKCS#7
            var tmp = signedCms.Encode();

            return Convert.ToBase64String(tmp);
        }

        private static bool VerifyMessageCms(string message)
        {
            byte[] signedMessage = Convert.FromBase64String(message);

            var signedCms = new GostSignedCms();

            signedCms.Decode(signedMessage);

            try { signedCms.CheckSignature(true); }
            catch (Exception ex) { Console.WriteLine(ex.Message); return false; }

            return true;
        }

        //public static void SignFullXml(string xmlFile, X509Certificate2 certificate, bool deleteSourceFile = false)
        //{
        //    var xmlDocument = new XmlDocument();
        //    xmlDocument.Load(new StringReader(File.ReadAllText(xmlFile)));

        //    var signedXml = new GostSignedXml(xmlDocument);
        //    signedXml.SetSigningCertificate(Certificate);

        //    var dataReference = new Reference { Uri = "", DigestMethod = GetDigestMethod(Certificate) };
        //    dataReference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

        //    signedXml.AddReference(dataReference); ;

        //    var keyInfo = new KeyInfo();
        //    keyInfo.AddClause(new KeyInfoX509Data(Certificate));
        //    signedXml.KeyInfo = keyInfo;

        //    signedXml.ComputeSignature();

        //    var signatureXml = signedXml.GetXml();

        //    xmlDocument.DocumentElement.AppendChild(xmlDocument.ImportNode(signatureXml, true));

        //    xmlDocument.Save(Path.Combine("C:\\_test\\signedFiles", ("Signed." + Path.GetFileName(xmlFile))));

        //    if (deleteSourceFile)
        //        File.Delete(Path.GetFullPath(xmlFile));
        //}

        //public static void SignFullXml(string[] arrXmlFiles, X509Certificate2 certificate, bool deleteSourceFile = false)
        //{
        //    Parallel.ForEach(arrXmlFiles,
        //        new ParallelOptions { MaxDegreeOfParallelism = -1 },
        //        xmlFile =>
        //        {
        //            var xmlDocument = new XmlDocument();
        //            xmlDocument.Load(new StringReader(File.ReadAllText(xmlFile)));

        //            var signedXml = new GostSignedXml(xmlDocument);
        //            signedXml.SetSigningCertificate(Certificate);

        //            var dataReference = new Reference { Uri = "", DigestMethod = GetDigestMethod(Certificate) };
        //            dataReference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

        //            signedXml.AddReference(dataReference); ;

        //            var keyInfo = new KeyInfo();
        //            keyInfo.AddClause(new KeyInfoX509Data(Certificate));
        //            signedXml.KeyInfo = keyInfo;

        //            signedXml.ComputeSignature();

        //            var signatureXml = signedXml.GetXml();

        //            xmlDocument.DocumentElement.AppendChild(xmlDocument.ImportNode(signatureXml, true));

        //            xmlDocument.Save(Path.Combine(StaticPathConfiguration.PathSignedFolder, ("Signed." + Path.GetFileName(xmlFile))));

        //            if (deleteSourceFile)
        //                File.Delete(Path.GetFullPath(xmlFile));
        //        });
        //}

        /// <summary> Получение объекта сертификата из хранилища </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static X509Certificate2 FindGostCertificate(Predicate<X509Certificate2> filter = null)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
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
        /// <summary>Ищет по ФИО и возвращает объект сертификата из хранилища</summary>
        /// <remarks>
        /// Если найти соотвествие не удалось, то возвращает null
        ///  NON USABLE
        /// </remarks>
        /// <param name="surname">Фамилия</param>
        /// <param name="name">Имя</param>
        /// <param name="lastname">Отчество</param>
        /// <returns>Возвращает объект сертификата из хранилища в формате X509Certificate2</returns>
        public static X509Certificate2 FindGostCurrentCertificate(string surname, string name, string lastname)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in store.Certificates)
            {
                Console.WriteLine();

                string certSurname, certName, CN = string.Empty;

                var fio = cert.Subject.Split('=')[5].Split(' ');
                Console.WriteLine();
                if (fio[0] == surname && fio[1] == name && fio[2] == lastname)
                    return cert;
            }

            return null;
        }


        /// <summary> Ищет сертификат по серийному номеру и возвращает объект сертификата из хранилища</summary>
        /// <remarks>Если найти соотвествие не удалось, то возвращает null</remarks>
        /// <param name="serialNumber">Серийный номер в виде строки (string)</param>
        /// <returns>Возвращает объект сертификата из хранилища в формате X509Certificate2</returns>
        public static X509Certificate2 FindGostCurrentCertificate(string serialNumber)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

            foreach (X509Certificate2 cert in store.Certificates)
                if (cert.SerialNumber.ToLower() == serialNumber.Replace(" ", "").ToLower())
                    return cert;

            return null;
        }

        /// <summary> Получение имени алгоритма </summary>
        /// <param name="certificate"> Передача сертификата из хранилища </param>
        /// <returns> Возвращает наименование алгоритма в виде строки типа string</returns>
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
