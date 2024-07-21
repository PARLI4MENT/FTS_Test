﻿using SQLTestNs;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace XmlNs
{
    class ImplementateToXml
    {
        public static string FileInFolder { get; private set; } = "C:\\_test\\intermidateFiles";
        public static string FileOutFolder { get; private set; } = "C:\\_test\\implementFiles";
        public static string FileTemplate { get; private set; } = "C:\\_test\\create_doc_in_arch.xml";

        public static string ExtractId(string pathToXML)
        {
            using (StreamReader sr = new StreamReader(pathToXML, true))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(sr);

                foreach (XmlNode xmlNode in xmlDoc.DocumentElement)
                {
                    if (xmlNode.Name == "DocumentID")
                    {
#if DEBUG
                        Console.WriteLine($"Node Name => {xmlNode.Name}");
                        Console.WriteLine($"Node InnerText => {xmlNode.InnerText}");
                        Console.WriteLine();
#endif
                        return xmlNode.InnerText;
                    }
                }
                return string.Empty;
            }
        }

        public static void ExtractId(string pathToXML, out string id, out string fileName)
        {
            if (String.IsNullOrEmpty(pathToXML))
            {
                Console.WriteLine("IS NULL OR EMPTY");
                id = null;
                fileName = null;
                return;
            }

            using (StreamReader sr = new StreamReader(pathToXML, true))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(sr);

                foreach (XmlNode xmlNode in xmlDoc.DocumentElement)
                {
                    if (xmlNode.Name == "DocumentID")
                    {
#if DEBUG
                        Console.WriteLine($"Node Name => {xmlNode.Name}");
                        Console.WriteLine($"Node InnerText => {xmlNode.InnerText}");
                        Console.WriteLine();
#endif
                        id = xmlNode.InnerText;
                        fileName = Path.GetFileName(pathToXML);
                        return;
                    }
                }
                id = null;
                fileName = null;

            }
        }

        public static void ImplementParallel(string[] implementFiles, bool deletedInputFile = false, int _MaxDegreeOfParallelism = -1)
        {
            Parallel.ForEach(implementFiles,
                new ParallelOptions { MaxDegreeOfParallelism = _MaxDegreeOfParallelism },
                implementFile =>
                {
                    const int Company_key_id = 1;

                    string DateStr = implementFile + ";";

                    //File.AppendAllText("C:\\_test\\Arch_docs.log", Environment.NewLine + "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

                    string NameArray = (string)Path.GetFileName(implementFile).Split('.')[0]; // Можно упростить
                    var file_xml = new XmlDocument();
                    var doc_to_arch = new XmlDocument();

                    string PrDocumentName = "", PrDocumentNumber = "", PrDocumentDate = "", DocCode = "", DocName = "";

                    string NewDocToArchName = Path.Combine(FileInFolder, Path.GetFileName(implementFile));
                    File.Copy(FileTemplate, NewDocToArchName, true);


                    file_xml.Load(new StringReader(File.ReadAllText(implementFile)));
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

                    File.Copy(NewDocToArchName, Path.Combine(FileOutFolder, Path.GetFileName(implementFile)), true);

                    //File.AppendAllText("C:\\_test\\Arch_docs.log", "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

                    // Send to DB
                    new PgSql().ExecuteToDB(new string[7] { NameArray, EnvelopeID, DocumentID, PrDocumentName, PrDocumentNumber, DocCode, NewDocToArchName }, Company_key_id);

                    if (deletedInputFile)
                    {
                        File.Delete(implementFile);
                        File.Delete(NewDocToArchName);
                    }
                });
        }

        private void DataImplementation(string FileName, bool deletedInputFile = false)
        {


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

            if (deletedInputFile)
            {
                File.Delete(FileName);
                File.Delete(NewDocToArchName);
            }
        }
    }
}
