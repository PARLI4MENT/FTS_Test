#define TEST 

using System.Xml;
using SqlTest;

namespace MainNs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /// Dont use
            #region
            //{
            //    var sw = new Stopwatch();
            //    sw.Restart();

            //    var thread = new Thread(() =>
            //    {
            //        PgInsertData("testTabel1", 0, 10000);
            //    });

            //    thread.Start();
            //    thread.Join();

            //    sw.Stop();
            //    Console.WriteLine(sw.Elapsed);
            //}
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


            //{
            //    string[] pathsFiles = Directory.GetFiles("C:\\_test\\ParseOutput");

            //    foreach (var filePath in pathsFiles)
            //    {
            //        ParseXML(filePath);
            //    }
            //}

            {
                ParseXML("C:\\_test\\_test\\1001204E\\00251779-b785-4cc1-92f9-8690174f14fa.92ec414c-ce5f-4cb8-a81c-cb62bcb60780.Passport.xml");
            }

            //#region Parse
            //{
            //    var renamer = new RenamerXML();
            //    renamer.ParseFileByMaskedParallel();
            //}
            //#endregion

            Console.ReadKey();
        }

        private static void ParseXML(string FileName)
        {
            string FileInFolder = "C:\\_test\\_test\\DOCS_TO_ARCH";
            const string FileTemplate = "C:\\_test\\create_doc_in_arch.xml";

            const int Company_key_id = 1;

            string DateStr = FileName + ";";

            File.AppendAllText( "C:\\_test\\Arch_docs.log", "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

            // Сделать String.Split
            // Переделать, забирает только первую часть [0]
            string NameArray = (string)Path.GetFileName(FileName).Split('.')[0];
            var file_xml = new XmlDocument();
            var doc_to_arch = new XmlDocument();

            string PrDocumentName, PrDocumentNumber, PrDocumentDate, DocCode, DocName;

            string NewDocToArchName = Path.Combine(FileInFolder, Path.GetFileName(FileName));
            File.Copy(FileTemplate, NewDocToArchName, true);

            /// Ошибка Unicode
            //file_xml.Load(pathToXmlFile);
            file_xml.Load(new StringReader(File.ReadAllText(FileName)));
            switch (file_xml.DocumentElement.GetAttribute("DocumentModeID"))
            {
                //'Договор ТамПред
                case "1006196E":
                    /*
                        PrDocumentName = DirectCast(file_xml.GetElementsByTagName("ContractDetails", "*")(0), XmlElement).GetElementsByTagName("PrDocumentName", "*")(0).InnerText
                        PrDocumentNumber = DirectCast(file_xml.GetElementsByTagName("ContractDetails", "*")(0), XmlElement).GetElementsByTagName("PrDocumentNumber", "*")(0).InnerText
                        PrDocumentDate = DirectCast(file_xml.GetElementsByTagName("ContractDetails", "*")(0), XmlElement).GetElementsByTagName("PrDocumentDate", "*")(0).InnerText
                        DocCode = "11002"
                        DocName = "Договор с ТамПред"
                     */
                    {
                        /// НУЖНО СДЕЛАТЬ !!
#if TEST
                        Console.WriteLine($"Path => [{FileName}]\nDocumentModeID => 1006196E");
                        CopyFile(FileName, "1006196E ");
#endif
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
                // Нет документа для теста
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
                // Нет документа для теста
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
                // Нет документа для теста
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

            string EnvelopeID = Guid.NewGuid().ToString().ToUpper();
            string DocumentID = Guid.NewGuid().ToString().ToUpper();
        }
        private static void CopyFile(string pathFile, string str = "")
        {
            File.Copy(pathFile, Path.Combine("C:\\_test\\_test", String.Concat(str, Path.GetFileName(pathFile))));

        }
    }
}