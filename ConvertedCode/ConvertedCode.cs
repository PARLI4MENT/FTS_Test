using Microsoft.VisualBasic;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Xml;

namespace MainNs
{
    public class Porgram
    {
        public static void Main(string[] args)
        {

        }

        public void TestPrepDocForArch(string FileName)
        {
            string FileInFolder = Application.StartupPath + @"\DOCS_TO_ARCH\";
            string FileOutFolder = Application.StartupPath + @"\OUT\";
            string FileTemplate = Application.StartupPath + @"\TEMPLATE_5_22\create_doc_in_arch.xml";
            int Company_key_id = 1;

            var d1 = DateTime.Now;
            var d2 = DateTime.Now;
            string DateStr = FileName + ";";

            //??
            //global::My.Computer.FileSystem.WriteAllText(Application.StartupPath + @"\Arch_docs.log", "New TEST;START;END CASE;PREP XML;SING XML;INSERT;", true);

            // DEBUG
            DateStr += Conversion.Str(Math.Round((DateTime.Now.Ticks - d2.Ticks) / 10000d, 0)) + ";";
            d2 = DateTime.Now;

            // Подготовка документов для отправки в архив
            string[] NameArray = Strings.Split(Path.GetFileName(FileName), ".");
            /// file_xml => Исходный файл
            var file_xml = new XmlDocument();
            /// doc_to_arch => Файл из шаблона
            var doc_to_arch = new XmlDocument();

            string PrDocumentName = "";
            string PrDocumentNumber = "";
            string PrDocumentDate = "";
            string DocCode = "";
            string DocName;

            string NewDocToArchName = FileInFolder + @"\" + Path.GetFileName(FileName);
            File.Copy(FileTemplate, NewDocToArchName, true);

            file_xml.Load(FileName);

            switch (file_xml.DocumentElement.GetAttribute("DocumentModeID"))
            {
                case "1006196E":    // Договор ТамПред
                    {
                        PrDocumentName = ((XmlElement)file_xml.GetElementsByTagName("ContractDetails", "*")(0)).GetElementsByTagName("PrDocumentName", "*")(0).InnerText;
                        PrDocumentNumber = ((XmlElement)file_xml.GetElementsByTagName("ContractDetails", "*")(0)).GetElementsByTagName("PrDocumentNumber", "*")(0).InnerText;
                        PrDocumentDate = ((XmlElement)file_xml.GetElementsByTagName("ContractDetails", "*")(0)).GetElementsByTagName("PrDocumentDate", "*")(0).InnerText;
                        DocCode = "11002";
                        DocName = "Договор с ТамПред";
                        break;
                    }
                case "1002008E":    // Довереность
                    {
                        PrDocumentName = file_xml.GetElementsByTagName("PrDocumentName")(0).InnerText;
                        PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber")(0).InnerText;
                        PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate")(0).InnerText;
                        DocCode = "11003";
                        DocName = "Доверенность";
                        break;
                    }
                case "1001204E":     // Паспорт
                    {
                        PrDocumentName = "Паспорт гражданина РФ";
                        PrDocumentNumber = file_xml.GetElementsByTagName("CardSeries")(0).InnerText;
                        PrDocumentNumber = PrDocumentNumber + " " + file_xml.GetElementsByTagName("CardNumber")(0).InnerText;
                        PrDocumentDate = file_xml.GetElementsByTagName("CardDate")(0).InnerText;
                        DocCode = "11001";
                        DocName = "Паспорт декларанта";
                        break;
                    }
                case "1002018E":     // Индивидуальная
                    {
                        PrDocumentName = "Индивидуальная накладная при экспресс перевозке";
                        PrDocumentNumber = file_xml.GetElementsByTagName("WayBillNumber")(0).InnerText;
                        if (file_xml.GetElementsByTagName("DateTime").Count > 0)
                        {
                            PrDocumentDate = file_xml.GetElementsByTagName("DateTime")(0).InnerText;
                        }
                        else
                        {
                            PrDocumentDate = "";
                        }
                        DocCode = "02021";
                        DocName = "Индивидуальная накладная";
                        file_xml.GetElementsByTagName("InternationalDistribution")(0).InnerText = 1;
                        break;
                    }
                case "1006110E":     // Графические материалы
                    {
                        PrDocumentName = file_xml.GetElementsByTagName("PrDocumentName")(0).InnerText;
                        PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber")(0).InnerText;
                        if (file_xml.GetElementsByTagName("PrDocumentDate").Count > 0)
                        {
                            if (file_xml.GetElementsByTagName("PrDocumentDate")(0).InnerText == "0001-01-01")
                            {
                                PrDocumentDate = "";
                            }
                            else
                            {
                                PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate")(0).InnerText;
                            }
                        }
                        DocCode = "09023";
                        DocName = "Графические материалы: " + PrDocumentName + " " + PrDocumentNumber;
                        file_xml.GetElementsByTagName("FileData")(0).InnerText = Strings.Replace(file_xml.GetElementsByTagName("FileData")(0).InnerText, '\r', "");
                        break;
                    }
                case "1006088E": // Текстовый документ
                    {
                        PrDocumentName = file_xml.GetElementsByTagName("DocumentName")(0).InnerText;
                        PrDocumentNumber = file_xml.GetElementsByTagName("DocumentNumber")(0).InnerText;
                        if (file_xml.GetElementsByTagName("DocumentDate").Count > 0)
                        {
                            PrDocumentDate = file_xml.GetElementsByTagName("DocumentDate")(0).InnerText;
                        }
                        if (PrDocumentName == "Экспертиза")
                        {
                            DocCode = "10023";
                        }
                        else
                        {
                            DocCode = "09999";
                        }
                        DocName = PrDocumentName;
                        break;
                    }
                case "1002007E": // Инвойс
                    {
                        PrDocumentName = "Инвойс";
                        PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber", "*")(0).InnerText;
                        if (file_xml.GetElementsByTagName("PrDocumentDate", "*").Count > 0)
                        {
                            PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate", "*")(0).InnerText;
                        }
                        DocCode = "04021";
                        DocName = PrDocumentName;
                        break;
                    }
                case "1002048E": // Расчет Утиль Сбора
                    {
                        PrDocumentName = "Расчет утилизационного сбора";
                        PrDocumentNumber = "";
                        PrDocumentDate = file_xml.GetElementsByTagName("CalculateDate", "*")(0).InnerText;
                        DocCode = "10064";
                        DocName = PrDocumentName;
                        break;
                    }
                case "1003202E": // Коносамент
                    {
                        PrDocumentName = "Коносамент";
                        PrDocumentNumber = file_xml.GetElementsByTagName("PrDocumentNumber", "*")(0).InnerText;
                        PrDocumentDate = file_xml.GetElementsByTagName("PrDocumentDate")(0).InnerText;
                        DocCode = "02011";
                        DocName = PrDocumentName;
                        break;
                    }
            }

            doc_to_arch.Load(NewDocToArchName);
            // DEBUG 
            //DateStr += Conversion.Str(Math.Round((DateTime.Now.Ticks - d2.Ticks) / 10000d, 0)) + ";";
            //d2 = DateTime.Now;
            XmlNode temp_node = doc_to_arch.ImportNode(file_xml.DocumentElement, true);
            doc_to_arch.GetElementsByTagName("Object")(1).AppendChild(temp_node);

            string EnvelopeID = Guid.NewGuid().ToString().ToUpper();
            string DocumentID = Guid.NewGuid().ToString().ToUpper();

            doc_to_arch.GetElementsByTagName("EnvelopeID", "*")(0).InnerText = EnvelopeID;
            doc_to_arch.GetElementsByTagName("roi:SenderInformation")(0).InnerText = MegaDict[Company_key_id]("Address");
            doc_to_arch.GetElementsByTagName("roi:PreparationDateTime")(0).InnerText = DateTime.Now.ToString("s") + DateTime.Now.ToString("zzz");
            doc_to_arch.GetElementsByTagName("ParticipantID")(0).InnerText = MegaDict[Company_key_id]("ParticipantID");
            doc_to_arch.GetElementsByTagName("CustomsCode")(0).InnerText = "10000000";
            doc_to_arch.GetElementsByTagName("X509Certificate")(0).InnerText = CompanyCert(MegaDict[Company_key_id]("CertSerialNum"));
            doc_to_arch.GetElementsByTagName("ct:DocumentID")(0).InnerText = Guid.NewGuid().ToString().ToUpper();
            doc_to_arch.GetElementsByTagName("ct:ArchDeclID")(0).InnerText = MegaDict[Company_key_id]("ParticipantID");
            doc_to_arch.GetElementsByTagName("ct:ArchID")(0).InnerText = MegaDict[Company_key_id]("ArchID");
            doc_to_arch.GetElementsByTagName("DocumentID", "*")(1).InnerText = DocumentID;
            doc_to_arch.GetElementsByTagName("DocCode", "*")(0).InnerText = DocCode;
            doc_to_arch.GetElementsByTagName("X509Certificate", "*")(1).InnerText = CompanyCert(MegaDict[Company_key_id]("CertSerialNum"));

            ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")(0)).GetElementsByTagName("PrDocumentName", "*")(0).InnerText = PrDocumentName;

            if (string.IsNullOrEmpty(PrDocumentNumber))
            {
                doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")(0).RemoveChild(((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")(0)).GetElementsByTagName("PrDocumentNumber", "*")(0));
            }
            else
            {
                ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")(0)).GetElementsByTagName("PrDocumentNumber", "*")(0).InnerText = PrDocumentNumber;
            }

            if (string.IsNullOrEmpty(PrDocumentDate) | string.IsNullOrEmpty(PrDocumentDate))
            {
                doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")(0).RemoveChild(((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")(0)).GetElementsByTagName("PrDocumentDate", "*")(0));
            }
            else
            {
                ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")(0)).GetElementsByTagName("PrDocumentDate", "*")(0).InnerText = PrDocumentDate;
            }

            doc_to_arch.Save(NewDocToArchName);

            // DEBUG
            DateStr += Conversion.Str(Math.Round((DateTime.Now.Ticks - d2.Ticks) / 10000d, 0)) + ";";
            d2 = DateTime.Now;

            // SignerXMLFile(NewDocToArchName, NewDocToArchName, Company_key_id)
            File.Copy(NewDocToArchName, FileOutFolder + Path.GetFileName(FileName), true);
            // DEBUG 
            DateStr += Conversion.Str(Math.Round((DateTime.Now.Ticks - d2.Ticks) / 10000d, 0)) + ";";

            myCmd.CommandText = "UPDATE ECD_list Set Status='Отправка в архив', DocsSended = DocsSended + 1 WHERE InnerID='" + NameArray[0] + "'";
            myCmd.ExecuteNonQuery();
            myCmd.CommandText = "INSERT INTO ExchED " + "(InnerID, MessageType, EnvelopeID, CompanySet_key_id, DocumentID, DocName, DocNum, DocCode, ArchFileName) " + " VALUES ('" + NameArray[0] + "', 'CMN.00202', '" + EnvelopeID + "', " + Company_key_id + " , '" + DocumentID + "' , '" + PrDocumentName + "' , '" + PrDocumentNumber + "' , '" + DocCode + "' , '" + ArchivePathDoc + Path.GetFileName(NewDocToArchName) + "')";
            myCmd.ExecuteNonQuery();

            // DEBUG 
            DateStr += Conversion.Str(Math.Round((DateTime.Now.Ticks - d2.Ticks) / 10000d, 0)) + ";";
            global::My.Computer.FileSystem.WriteAllText(Application.StartupPath + @"\Arch_docs.log", DateStr, true);
            global::My.Computer.FileSystem.WriteAllText(Application.StartupPath + @"\Arch_docs.log", "\n" + '\r', true);
            File.Delete(FileName);
            File.Delete(NewDocToArchName);
        }
    }

}