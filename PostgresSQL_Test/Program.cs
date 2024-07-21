#define TEST 

using Npgsql;

using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using OpenGost.Security.Cryptography;

namespace XMLSigner
{ 
    public class Program
    {
        public static void Main(string[] args)
        {
            #region
            //const string _strMssqlCreateTable = @"CREATE TABLE [mainScheme].[Untitled] (
            //  [InnerID] nvarchar(255) NULL,
            //  [MessageType] nvarchar(255) NULL,
            //  [EnvelopeID] nvarchar(255) NOT NULL,
            //  [CompanySet_key_id] int NULL,
            //  [DocumentID] nvarchar(255) NULL,
            //  [DocName] nvarchar(255) NULL,
            //  [DocNum] nvarchar(255) NULL,
            //  [DocCode] nvarchar NULL,
            //  [ArchFileName] varbinary(max) NULL,
            //  PRIMARY KEY CLUSTERED ([EnvelopeID])
            //    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
            //    )";
            #endregion

            /// Dont use
            #region
            //{
            //    var sw = new Stopwatch();
            //    sw.Start();
            //    sw.Restart();

            //    Task th = Task.Factory.StartNew(() => PgInsertData("testTabel1", 10000));
            //    Task.WhenAll(th).ContinueWith(task => sw.Stop());

            //    Console.WriteLine(sw.Elapsed);

            //}
            #endregion

            #region Renaming rawFiles
            //Console.WriteLine("Start process...");
            //SqlTest.RenamerXML renamerXML = new SqlTest.RenamerXML();
            //renamerXML.RenameAndMoveParallel();
            #endregion

            #region MSSQL Test
            //{
            //    string _strMssqlConn = @"Server=192.168.0.142,1433;Database=master;User Id=SA;Password=P&sswd0105;Encrypt=True;TrustServerCertificate=True";
            //    try
            //    {
            //        using (var sqlConn = new SqlConnection(_strMssqlConn))
            //        {
            //            Console.WriteLine(sqlConn.State.ToString());
            //            using (var sqlComm = sqlConn.CreateCommand())
            //            {

            //            }
            //            sqlConn.Close();
            //        }

            //    }
            //    catch (Exception ex) { Console.WriteLine(ex.Message); return; }
            //}
            #endregion

            #region Implementation Aria Test
            {
                //Console.WriteLine("Start...");
                //var swInplement = new Stopwatch();
                //swInplement.Start();

                //string[] allFiles = Directory.GetFiles("C:\\_test\\intermidateFiles");

                //Parallel.ForEach(allFiles, new ParallelOptions { MaxDegreeOfParallelism = -1 },
                //    file => { DataImplementation(file, true); });

                //swInplement.Stop();
                //Console.WriteLine($"\nTotal time: {swInplement.ElapsedMilliseconds / 1000},{swInplement.ElapsedMilliseconds % 1000} sec");
                //Console.WriteLine($"Total files ({Directory.GetFiles("C:\\_test\\implementFiles").Count()} " +
                //    $"/ {Directory.GetFiles("C:\\_test\\intermidateFiles").Count()})");
                //Console.WriteLine($"AVG time: {allFiles.Count() / (int)(swInplement.ElapsedMilliseconds / 1000)}," +
                //    $"{(allFiles.Count() / (int)swInplement.ElapsedMilliseconds) % 1000} units");
            }
            #endregion

            #region Encrypt xml
            {
                OpenGostCryptoConfig.ConfigureCryptographicServices();
                var certificate = FindGostCertificate();

                //if (Directory.GetFiles("C:\\_test\\signedFiles").Count() > 0)
                //{
                //    string[] listSignFiles = Directory.GetFiles("C:\\_test\\signingFiles");
                //    foreach (string signFile in listSignFiles)
                //        File.Delete(signFile);
                //}

                //string pathToFile = "C:\\_test\\EncryptedXmlExample.xml";
                //var signedXml = SignXmlDocument(pathToFile, ref certificate);
                //signedXml.Save(Path.Combine("C:\\_test\\signedFiles", "Signed." + Path.GetFileName(pathToFile)));
            }
            #endregion

            Console.ReadKey();
        }

        public static X509Certificate2 FindGostCertificate(StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.LocalMachine, Predicate<X509Certificate2> filter = null)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

            try
            {
                return store.Certificates[0];
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Заполнение, подписание и отправка в БД
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="deletedInputFile"></param>
        private static void DataImplementation(string FileName, bool deletedInputFile = false)
        {

            string FileInFolder = "C:\\_test\\intermidateFiles";
            string FileOutFolder = "C:\\_test\\implementFiles";
            const string FileTemplate = "C:\\_test\\create_doc_in_arch.xml";

            const int Company_key_id = 1;

            string DateStr = FileName + ";";

            //File.AppendAllText("C:\\_test\\Arch_docs.log", Environment.NewLine + "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

            string NameArray = (string)Path.GetFileName(FileName).Split('.')[0];
            var file_xml = new XmlDocument();
            var doc_to_arch = new XmlDocument();

            string? PrDocumentName = "", PrDocumentNumber = "", PrDocumentDate = "", DocCode = "", DocName = "";

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

            #region SignXmlFile
            //SignerXMLFile(NewDocToArchName, NewDocToArchName2, Company_key_id);
            {

            }
            #endregion

            File.Copy(NewDocToArchName, Path.Combine(FileOutFolder, Path.GetFileName(FileName)), true);

            //File.AppendAllText("C:\\_test\\Arch_docs.log", "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

            //Send to PostgresSQL DB
            //string _strConnMain = $"Server=192.168.0.142;Port=5438;Uid=postgres;Pwd=passwd0105;Database=declarantplus;" +
            //    $"Connection Idle Lifetime=20;Maximum Pool Size=150;";
            //string _strConnMain = $"Server=localhost;Port=5432;Uid=postgres;Pwd=passwd0105;Database=declarantplus;";
            //using (var sqlConn = new NpgsqlConnection(_strConnMain))
            //{
            //    sqlConn.Open();
            //    using (var sqlComm = new Npgsql.NpgsqlCommand())
            //    {
            //        sqlComm.CommandText = $@"INSERT INTO ""public"".""ExchED""
            //                    (""InnerID"", ""MessageType"", ""EnvelopeID"", ""CompanySet_key_id"",
            //                    ""DocumentID"", ""DocName"", ""DocNum"", ""DocCode"", ""ArchFileName"")
            //                    VALUES ('{NameArray}', 'CMN.00202', '{EnvelopeID}', {Company_key_id}, '{DocumentID}',
            //                    '{PrDocumentName}', '{PrDocumentNumber}', '{DocCode}', '{Path.GetFileName(NewDocToArchName)}');";
            //        sqlComm.Connection = sqlConn;
            //        sqlComm.ExecuteNonQuery();
            //        // ArchivePathDoc ???
            //    }
            //    sqlConn.Close();
            //}

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
        
        //public static XmlDocument SignXmlDocument(string pathToXmlFile, ref X509Certificate2 certificate)
        //{
        //    var xmlDocument = new XmlDocument();
        //    xmlDocument.Load(new StringReader(File.ReadAllText(pathToXmlFile)));

        //    //var signedXml = new GostSignedXml(xmlDocument);
        //    var signedXml = new XmlDocument();
        //    signedXml.SetSigningCertificate(certificate);

        //    var dataReference = new Reference { Uri = "", DigestMethod = GetDigestMethod(certificate) };
        //    dataReference.AddTransform(new XmlDsigEnvelopedSignatureTransform());


        //    signedXml.AddReference(dataReference); ;

        //    var keyInfo = new KeyInfo();
        //    keyInfo.AddClause(new KeyInfoX509Data(certificate));
        //    signedXml.KeyInfo = keyInfo;

        //    signedXml.ComputeSignature();

        //    var signatureXml = signedXml.GetXml();

        //    xmlDocument.DocumentElement.AppendChild(xmlDocument.ImportNode(signatureXml, true));
        //    return xmlDocument;
        //}

        private static string GetDigestMethod(X509Certificate2 certificate)
        {
            //using (var publicKey = (GostAsymmetricAlgorithm)certificate.GetPrivateKeyAlgorithm())
            //{
            //    using (var hasAlgorithm = publicKey.CreateHashAlgorithm())
            //    {
            //        return hasAlgorithm.AlgorithmName;
            //    }
            //}
            return string.Empty;
        }

        /// <summary>
        /// Для внутренего использования
        /// </summary>
        private static void NpgSqlTest()
        {
            string _strConnMain = $"Server=192.168.0.142;Port=5438;Uid=postgres;Pwd=passwd0105;Database=declarantplus;";
            try
            {
                using (var sqlConn = new NpgsqlConnection(_strConnMain))
                {
                    sqlConn.Open();
                    using (var sqlComm = new Npgsql.NpgsqlCommand())
                    {
                        sqlComm.CommandText = $@"INSERT INTO ""public"".""ExchED""
                                (""InnerID"", ""MessageType"", ""EnvelopeID"", ""CompanySet_key_id"",
                                ""DocumentID"", ""DocName"", ""DocNum"", ""DocCode"", ""ArchFileName"")
                                VALUES ('NameArray', 'CMN.00202', '{Guid.NewGuid().ToString()}', 1, 'DocumentID',
                                'PrDocumentName', 'PrDocumentNumber', 'DocCode', 'Path.GetFileName(NewDocToArchName)');";
                        sqlComm.Connection = sqlConn;
                        sqlComm.ExecuteNonQuery();
                        // ArchivePathDoc ???
                    }
                    sqlConn.Close();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); Console.ReadKey(); }
        }
    }
}