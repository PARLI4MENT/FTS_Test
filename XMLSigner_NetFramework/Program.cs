#define TEST

using GostCryptography.Base;
using GostCryptography.Xml;
using Microsoft.Win32.SafeHandles;
using FileNs;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Linq;
using Npgsql;
using SQLTest;

namespace XMLSigner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PgSql pgSql = new PgSql("192.168.0.142", "5438", "postgres", "passwd0105");



            // Setting cert to variable
            var certificate = FindGostCertificate();

            Console.WriteLine("Start process...");

            // Rename and move to intermidateFiles XML files
            //string[] rowFiles = Directory.GetDirectories("C:\\_test\\rawFiles");
            //Parallel.ForEach(rowFiles, rowFile =>
            //{
            //    string[] subDir = Directory.GetDirectories(Path.Combine(rowFile, "files"));

            //    string[] filesSubfolder = (Directory.GetFiles(Path.Combine(subDir[0], "xml")));

            //    foreach (string file in filesSubfolder)
            //    {
            //        Task.Run(() =>
            //        {
            //            // Combine Xml
            //            string tmpPathCombine = Path.Combine("C:\\_test\\intermidateFiles", string.Concat(Path.GetFileName(rowFile), ".", Path.GetFileName(file)));
            //            File.Copy(file, tmpPathCombine);
            //        });
            //    }
            //});

            var sw = new Stopwatch();
            var swTotal = new Stopwatch();
            sw.Start();
            swTotal.Start();

            // Inplement to XML, signing and sending request to BD
            Console.WriteLine("Start inplement...");
            string[] implementFiles = Directory.GetFiles("C:\\_test\\intermidateFiles");
            Parallel.ForEach(implementFiles,
                new ParallelOptions { MaxDegreeOfParallelism = -1 },
                implementFile =>
                {
                    DataImplementation(implementFile);
                });
            sw.Stop();
            Console.WriteLine($"Time inplement => {sw.ElapsedMilliseconds / 1000},{sw.ElapsedMilliseconds % 1000} sec");
            Console.WriteLine($"Total destination files => {Directory.GetFiles("C:\\_test\\implementFiles").Count()} units");
            Console.WriteLine($"AVG => {(Directory.GetFiles("C:\\_test\\intermidateFiles").Count()) / ((int)sw.ElapsedMilliseconds / 1000)}");

            sw.Restart();
            Console.WriteLine("\nStart signing XML...");
            string[] singingFiles = Directory.GetFiles("C:\\_test\\implementFiles");
            Parallel.ForEach(singingFiles,
                new ParallelOptions { MaxDegreeOfParallelism = -1},
                singFile => {
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
            Console.ReadKey();
        }

        private static void DataImplementation(string FileName, bool deletedInputFile = false)
        {

            string FileInFolder = "C:\\_test\\intermidateFiles";
            string FileOutFolder = "C:\\_test\\implementFiles";
            const string FileTemplate = "C:\\_test\\create_doc_in_arch.xml";

            const int Company_key_id = 1;

            string DateStr = FileName + ";";

            //File.AppendAllText("C:\\_test\\Arch_docs.log", Environment.NewLine + "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

            string NameArray = (string)Path.GetFileName(FileName).Split('.')[0]; // Можно упростить
            var file_xml = new XmlDocument();
            var doc_to_arch = new XmlDocument();

            string PrDocumentName = "", PrDocumentNumber = "", PrDocumentDate = "", DocCode = "", DocName = "";

            string NewDocToArchName = Path.Combine(FileInFolder, Path.GetFileName(FileName));
            File.Copy(FileTemplate, NewDocToArchName, true);


            file_xml.Load(new StringReader(File.ReadAllText(FileName)));
            switch (file_xml.DocumentElement.GetAttribute("DocumentModeID"))
            {
                //Договор ТамПред
                case "1006196E":
                    {
                        PrDocumentName = ((XmlElement)file_xml.GetElementsByTagName("ContractDetails", "*")[0]).GetElementsByTagName("PrDocumentName", "*")[0].InnerText;
                        PrDocumentNumber = ((XmlElement)file_xml.GetElementsByTagName("ContractDetails", "*")[0]).GetElementsByTagName("PrDocumentNumber", "*")[0].InnerText;
                        PrDocumentDate = ((XmlElement)file_xml.GetElementsByTagName("ContractDetails", "*")[0]).GetElementsByTagName("PrDocumentDate", "*")[0].InnerText;
                        DocCode = "11002";
                        DocName = "Договор с ТамПред";
                    }
                    break;

                //Довереность
                case "1002008E":
                    {
                        PrDocumentName = file_xml.GetElementsByTagName("PrDocumentName")[0].InnerText;
                        PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber")[0].InnerText;
                        PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate")[0].InnerText;
                        DocCode = "11003";
                        DocName = "Доверенность";
                    }
                    break;

                //Паспорт
                case "1001204E":
                    {
                        PrDocumentName = "Паспорт гражданина РФ";
                        PrDocumentNumber = $"{file_xml.GetElementsByTagName("CardSeries")[0].InnerText}" +
                            $" {file_xml.GetElementsByTagName("CardNumber")[0].InnerText}";
                        PrDocumentDate = file_xml.GetElementsByTagName("CardDate")[0].InnerText;
                        DocCode = "11001";
                        DocName = "Паспорт декларанта";
                    }
                    break;

                //Индивидуальная
                case "1002018E":
                    {
                        PrDocumentName = "Индивидуальная накладная при экспресс перевозке";
                        PrDocumentNumber = file_xml.GetElementsByTagName("WayBillNumber")[0].InnerText;
                        if (file_xml.GetElementsByTagName("DateTime").Count > 0)
                            PrDocumentDate = file_xml.GetElementsByTagName("DateTime")[0].InnerText;
                        else
                            PrDocumentDate = "";
                        DocCode = "02021";
                        DocName = "Индивидуальная накладная";
                        file_xml.GetElementsByTagName("InternationalDistribution")[0].InnerText = "1";
                    }
                    break;

                //Графические материалы
                case "1006110E":
                    {
                        PrDocumentName = file_xml.GetElementsByTagName("PrDocumentName")[0].InnerText;
                        PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber")[0].InnerText;
                        PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate")[0].InnerText;
                        DocCode = "09023";
                        DocName = $"Графические материалы: {PrDocumentName} {PrDocumentNumber}";
                        //file_xml.GetElementsByTagName("FileData")(0).InnerText 
                        //    = Replace(file_xml.GetElementsByTagName("FileData")(0).InnerText, Chr(13), "")
                        file_xml.GetElementsByTagName("FileData")[0].InnerText =
                            file_xml.GetElementsByTagName("FileData")[0].InnerText.Replace("\r", "");
                    }
                    break;

                //Текстовый документ
                case "1006088E":
                    {
                        PrDocumentName = file_xml.GetElementsByTagName("DocumentName")[0].InnerText;
                        PrDocumentNumber = file_xml.GetElementsByTagName("DocumentNumber")[0].InnerText;
                        if (file_xml.GetElementsByTagName("DocumentDate").Count > 0)
                            if (PrDocumentName == "Экспертиза")
                                DocCode = "10023";
                            else
                                DocCode = "09999";
                        DocName = PrDocumentName;
                    }
                    break;

                //Инвойс
                case "1002007E":
                    {
                        PrDocumentName = "Инвойс";
                        PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber", "*")[0].InnerText;
                        if (file_xml.GetElementsByTagName("PrDocumentDate", "*").Count > 0)
                            PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate", "*")[0].InnerText;
                        DocCode = "04021";
                        DocName = PrDocumentName;
                    }
                    break;

                //Расчет Утиль Сбора
                case "1002048E":
                    {
                        PrDocumentName = "Расчет утилизационного сбора";
                        PrDocumentNumber = "";
                        PrDocumentDate = file_xml.GetElementsByTagName("CalculateDate", "*")[0].InnerText;
                        DocCode = "10064";
                        DocName = PrDocumentName;
                    }
                    break;

                //Коносамент
                case "1003202E":
                    {
                        PrDocumentName = "Коносамент";
                        PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber", "*")[0].InnerText;
                        PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate")[0].InnerText;
                        DocCode = "02011";
                        DocName = PrDocumentName;
                    }
                    break;
            }

            doc_to_arch.Load(NewDocToArchName);

            var temp_node = doc_to_arch.ImportNode(file_xml.DocumentElement, true);
            doc_to_arch.GetElementsByTagName("Object")[1].AppendChild(temp_node);

            string EnvelopeID = Guid.NewGuid().ToString().ToUpper();
            string DocumentID = Guid.NewGuid().ToString().ToUpper();

            doc_to_arch.GetElementsByTagName("EnvelopeID", "*")[0].InnerText = EnvelopeID;
            doc_to_arch.GetElementsByTagName("roi:SenderInformation")[0].InnerText = "SenderInformation_TEMP";
            doc_to_arch.GetElementsByTagName("roi:PreparationDateTime")[0].InnerText = DateTime.Now.ToString("s") + DateTime.Now.ToString("zzz");
            doc_to_arch.GetElementsByTagName("ParticipantID")[0].InnerText = "ParticipantID";
            doc_to_arch.GetElementsByTagName("CustomsCode")[0].InnerText = "10000000";
            doc_to_arch.GetElementsByTagName("X509Certificate")[0].InnerText = "X509Certificate_TEMP";
            doc_to_arch.GetElementsByTagName("ct:DocumentID")[0].InnerText = (Guid.NewGuid().ToString()).ToUpper();
            doc_to_arch.GetElementsByTagName("ct:ArchDeclID")[0].InnerText = "ArchDeclID_TEMP";
            doc_to_arch.GetElementsByTagName("ct:ArchID")[0].InnerText = "ArchID_TEMP";
            doc_to_arch.GetElementsByTagName("DocumentID", "*")[1].InnerText = DocumentID;
            doc_to_arch.GetElementsByTagName("DocCode", "*")[0].InnerText = DocCode;
            doc_to_arch.GetElementsByTagName("X509Certificate", "*")[0].InnerText = "X509Certificate_TEMP *";

            ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentName", "*")[0].InnerText = PrDocumentName;

            if (string.IsNullOrEmpty(PrDocumentNumber))
                doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0].RemoveChild(((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentNumber", "*")[0]);
            else
                ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentNumber", "*")[0].InnerText = PrDocumentNumber;

            if (string.IsNullOrEmpty(PrDocumentDate))
                doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0].RemoveChild(((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentDate", "*")[0]);
            else
                ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentDate", "*")[0].InnerText = PrDocumentDate;

            doc_to_arch.Save(NewDocToArchName);

            File.Copy(NewDocToArchName, Path.Combine(FileOutFolder, Path.GetFileName(FileName)), true);

            //File.AppendAllText("C:\\_test\\Arch_docs.log", "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

            //Send to PostgresSQL DB
            //string _strConnMain = $"Server=192.168.0.142;Port=5438;Uid=postgres;Pwd=passwd0105;Database=declarantplus;" +
            //    $"Connection Idle Lifetime=20;Maximum Pool Size=150;";
            string _strConnMain = $"Server=192.168.0.142;Port=5438;Uid=postgres;Pwd=passwd0105;Database=declarantplus;";
            using (var sqlConn = new NpgsqlConnection(_strConnMain))
            {
                sqlConn.Open();
                using (var sqlComm = new Npgsql.NpgsqlCommand())
                {
                    sqlComm.CommandText = $@"INSERT INTO ""public"".""ExchED""
                                (""InnerID"", ""MessageType"", ""EnvelopeID"", ""CompanySet_key_id"",
                                ""DocumentID"", ""DocName"", ""DocNum"", ""DocCode"", ""ArchFileName"")
                                VALUES ('{NameArray}', 'CMN.00202', '{EnvelopeID}', {Company_key_id}, '{DocumentID}',
                                '{PrDocumentName}', '{PrDocumentNumber}', '{DocCode}', '{Path.GetFileName(NewDocToArchName)}');";
                    sqlComm.Connection = sqlConn;
                    sqlComm.ExecuteNonQuery();
                    // ArchivePathDoc ???
                }
                sqlConn.Close();
            }

            #region
            //await using (var sqlConn = new NpgsqlConnection(_strConnMain))
            //{
            //    await sqlConn.OpenAsync();
            //    await using (var comm = new Npgsql.NpgsqlCommand($@"INSERT INTO ""public"".""ExchED""
            //                (""InnerID"", ""MessageType"", ""EnvelopeID"", ""CompanySet_key_id"",
            //                ""DocumentID"", ""DocName"", ""DocNum"", ""DocCode"", ""ArchFileName"")
            //                VALUES ('{NameArray}', 'CMN.00202', '{EnvelopeID}', {Company_key_id}, '{DocumentID}',
            //                '{PrDocumentName}', '{PrDocumentNumber}', '{DocCode}', '{Path.GetFileName(NewDocToArchName)}');", sqlConn))
            //    //(""InnerID"", ""Status"", ""DocsSended"") VALUES ('{NameArray}', 'Отправка в архив', 1)", sqlConn))
            //    {
            //        await comm.ExecuteNonQueryAsync();
            //        // ArchivePathDoc ???
            //    }
            //}
            #endregion

            if (deletedInputFile)
            {
                File.Delete(FileName);
                File.Delete(NewDocToArchName);
            }
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


        public void ImplementationToXML()
        { 
        
        }
    }
}
