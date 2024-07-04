using SqlTest;
using SQLTest;
using System.Configuration;
using System.Xml;

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


            #region Parse internal XML files
            {
                string pathToInputFile = "C:\\_test\\OUT\\0be68d4a-444d-4abb-a09f-ce07c9256e30.1f2aa4ac-e439-45f6-b4ce-0a21b4f9fcb9.FreeBinaryDoc.xml";
                const string pathTemplate = "C:\\_test\\create_doc_in_arch.xml";

                var file_xml = new XmlDocument();
                var archiveXmlDoc = new XmlDocument();

                string _doneXmlFile = "C:\\_test\\intermidate_folder";
                File.Copy(pathTemplate, Path.Combine(_doneXmlFile, Path.GetFileName(pathToInputFile)), true);


                string PrDocumentName, PrDocumentNumber, PrDocumentDate, DocCode, DocName;

                file_xml.Load(pathToInputFile);
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

                        }
                        break;

                    //Довереность
                    case "1002008E":
                        /*
                            PrDocumentName = file_xml.GetElementsByTagName("PrDocumentName")(0).InnerText
                            PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber")(0).InnerText
                            PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate")(0).InnerText
                            DocCode = "11003"
                            DocName = "Доверенность"
                         */
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
                        /*
                            PrDocumentName = "Паспорт гражданина РФ"
                            PrDocumentNumber = file_xml.GetElementsByTagName("CardSeries")(0).InnerText
                            PrDocumentNumber = PrDocumentNumber & " " & file_xml.GetElementsByTagName("CardNumber")(0).InnerText
                            PrDocumentDate = file_xml.GetElementsByTagName("CardDate")(0).InnerText
                            DocCode = "11001"
                            DocName = "Паспорт декларанта"
                         */
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
                        /*
                            PrDocumentName = "Индивидуальная накладная при экспресс перевозке"
                            PrDocumentNumber = file_xml.GetElementsByTagName("WayBillNumber")(0).InnerText
                            If file_xml.GetElementsByTagName("DateTime").Count > 0 Then
                                PrDocumentDate = file_xml.GetElementsByTagName("DateTime")(0).InnerText
                            Else
                                PrDocumentDate = ""
                            End If
                            DocCode = "02021"
                            DocName = "Индивидуальная накладная"
                            file_xml.GetElementsByTagName("InternationalDistribution")(0).InnerText = 1
                         */
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
                        /*
                            PrDocumentName = file_xml.GetElementsByTagName("PrDocumentName")(0).InnerText
                            PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber")(0).InnerText
                            If file_xml.GetElementsByTagName("PrDocumentDate").Count > 0 Then
                                If file_xml.GetElementsByTagName("PrDocumentDate")(0).InnerText = "0001-01-01" Then
                                    PrDocumentDate = ""
                            Else
                                PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate")(0).InnerText
                            End If
                            End If
                            DocCode = "09023"
                            DocName = "Графические материалы: " & PrDocumentName & " " & PrDocumentNumber
                            file_xml.GetElementsByTagName("FileData")(0).InnerText = Replace(file_xml.GetElementsByTagName("FileData")(0).InnerText, Chr(13), "")
                         */
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
                        /*
                            PrDocumentName = file_xml.GetElementsByTagName("DocumentName")(0).InnerText
                            PrDocumentNumber = file_xml.GetElementsByTagName("DocumentNumber")(0).InnerText
                            If file_xml.GetElementsByTagName("DocumentDate").Count > 0 Then
                                PrDocumentDate = file_xml.GetElementsByTagName("DocumentDate")(0).InnerText
                            End If
                            If PrDocumentName = "Экспертиза" Then
                                DocCode = "10023"
                            Else
                                DocCode = "09999"
                            End If
                            DocName = PrDocumentName
                         */
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
                        /*
                            PrDocumentName = "Инвойс"
                            PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber", "*")(0).InnerText
                            If file_xml.GetElementsByTagName("PrDocumentDate", "*").Count > 0 Then
                                PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate", "*")(0).InnerText
                            End If
                            DocCode = "04021"
                            DocName = PrDocumentName
                         */
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
                        /*
                            PrDocumentName = "Расчет утилизационного сбора"
                            PrDocumentNumber = ""
                            PrDocumentDate = file_xml.GetElementsByTagName("CalculateDate", "*")(0).InnerText
                            DocCode = "10064"
                            DocName = PrDocumentName
                         */
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
                        /*
                            PrDocumentName = "Коносамент"
                            PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber", "*")(0).InnerText
                            PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate")(0).InnerText
                            DocCode = "02011"
                            DocName = PrDocumentName
                         */
                        {
                            PrDocumentName = "Коносамент";
                            PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber", "*")[0].InnerText;
                            PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate")[0].InnerText;
                            DocCode = "02011";
                            DocName = PrDocumentName;
                        }
                        break;
                }

                archiveXmlDoc.Load(Path.Combine(_doneXmlFile, Path.GetFileName(pathToInputFile)));

                string EnvelopeID = Guid.NewGuid().ToString().ToUpper();
                string DocumentID = Guid.NewGuid().ToString().ToUpper();
            }
            #endregion

            #region Parse
            //{
            //    var renamer = new RenamerXML();
            //    renamer.Delete();
            //    //renamer.ParseFileByMaskedParallel();
            //}
            #endregion

            Console.ReadKey();
        }
    }
};