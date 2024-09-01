using SQLNs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using XmlFTS.OutClass;

namespace XmlNs
{
    public class ImplementateToXml
    {
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

        /// <summary> Извлечение данные из промежуточных XML-файлов, вставка в шаблонный XML и сохранение в папку (по-умолчанию implementFiles ) </summary>
        /// <param name="intermidateFile"> Путь к XML-файлу</param>
        /// <returns></returns>
        public static string ImplementLinear(string intermidateFile)
        {
            string DateStr = intermidateFile + ";";

            //File.AppendAllText("C:\\_test\\Arch_docs.log", Environment.NewLine + "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

            string NameArray = (string)Path.GetFileName(intermidateFile).Split('.')[0]; // Можно упростить
            var file_xml = new XmlDocument();
            var doc_to_arch = new XmlDocument();

            string PrDocumentName = "", PrDocumentNumber = "", PrDocumentDate = "", DocCode = "", DocName = "";

            string NewDocToArchName = Path.Combine(StaticPathConfiguration.PathIntermidateFolder, Path.GetFileName(intermidateFile));
            File.Copy(StaticPathConfiguration.TemplateXML, NewDocToArchName, true);

            file_xml.Load(new StringReader(File.ReadAllText(intermidateFile)));
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

            doc_to_arch.Load(intermidateFile);

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

            File.Copy(NewDocToArchName, Path.Combine(StaticPathConfiguration.PathImplementFolder, Path.GetFileName(intermidateFile)), true);

            //File.AppendAllText("C:\\_test\\Arch_docs.log", "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

            // Query to PostgresSql DB
            PgSql.SetConnectionString("localhost", "5438", "postgres", "passwd0105");
            new PgSql().ExecuteToDB(new string[7] { NameArray, EnvelopeID, DocumentID, PrDocumentName, PrDocumentNumber, DocCode, NewDocToArchName });

            if (Config.DeleteSourceFiles)
            {
                File.Delete(intermidateFile);
                File.Delete(NewDocToArchName);
            }

            return string.Empty;
        }

        /// <summary> Извлечение данные из промежуточных XML-файлов, вставка в шаблонный XML и сохранение в папку (implementFiles по-умолчанию)</summary>
        /// <param name="intermidateFiles"> Принимает массив строк (string[]) с путями к файлам </param>
        /// <param name="_MaxDegreeOfParallelism"> Лимит параллельных операций (По-умолчанию -1 (ограничение отсутствует)) </param>
        /// <param name="deletedInputFile"> Удалять исходные XML-файлы (по-умолчанию false) </param>
        /// <returns></returns>
        public static List<string[]> ImplementParallel(string[] intermidateFiles)
        {
            List<string[]> doneData = new List<string[]>();
            Parallel.ForEach(
                intermidateFiles,
                new ParallelOptions { MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism },
                intermidateFile =>
                {
                    int Company_key_id = 1;

                    string DateStr = intermidateFile + ";";

                    //File.AppendAllText("C:\\_test\\Arch_docs.log", Environment.NewLine + "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

                    string NameArray = Path.GetFileName(intermidateFile).Split('.')[0]; // Можно упростить
                    var file_xml = new XmlDocument();
                    var doc_to_arch = new XmlDocument();

                    string PrDocumentName = "", PrDocumentNumber = "", PrDocumentDate = "", DocCode = "", DocName = "";

                    string NewDocToArchName = Path.Combine(StaticPathConfiguration.PathIntermidateFolder, Path.GetFileName(intermidateFile));
                    File.Copy(StaticPathConfiguration.TemplateXML, NewDocToArchName, true);


                    file_xml.Load(new StringReader(File.ReadAllText(intermidateFile)));
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

                    File.Copy(NewDocToArchName, Path.Combine(StaticPathConfiguration.PathImplementFolder, Path.GetFileName(intermidateFile)), true);

                    //File.AppendAllText("C:\\_test\\Arch_docs.log", "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

                    // Query to PostgresSql DB
                    new PgSql().ExecuteToDB(new string[7] { NameArray, EnvelopeID, DocumentID, PrDocumentName, PrDocumentNumber, DocCode, NewDocToArchName });

                    doneData.Add(new string[7] { NameArray, EnvelopeID, DocumentID, PrDocumentName, PrDocumentNumber, DocCode, NewDocToArchName });

                    if (Config.DeleteSourceFiles)
                    {
                        File.Delete(intermidateFile);
                        File.Delete(NewDocToArchName);
                    }
                });
            return doneData;
        }

        /// <summary> Извлечение данные из промежуточных XML-файлов, вставка в шаблонный XML и сохранение в папку (implementFiles по-умолчанию)</summary>
        /// <param name="intermidateFiles"> Set intermidate array files</param>
        /// <param name="_MaxDegreeOfParallelism"> Лимит параллельных операций (По-умолчанию -1 (ограничение отсутсвует)) </param>
        /// <param name="deletedInputFile">Delete src XML-file</param>
        public static void ImplementParallel(string intermidateFolder)
        {
            string[] intermidateFiles = Directory.GetFiles(intermidateFolder, "*.xml", SearchOption.AllDirectories);
            Parallel.ForEach(intermidateFiles,
            new ParallelOptions { MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism },
            intermidateFile =>
            {
                int Company_key_id = 1;

                string DateStr = intermidateFile + ";";

                //File.AppendAllText("C:\\_test\\Arch_docs.log", Environment.NewLine + "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

                string NameArray = Path.GetFileName(intermidateFile).Split('.')[0]; // Можно упростить
                var file_xml = new XmlDocument();
                var doc_to_arch = new XmlDocument();

                string PrDocumentName = "", PrDocumentNumber = "", PrDocumentDate = "", DocCode = "", DocName = "";

                string NewDocToArchName = Path.Combine(StaticPathConfiguration.PathIntermidateFolder, Path.GetFileName(intermidateFile));
                File.Copy(StaticPathConfiguration.TemplateXML, NewDocToArchName, true);


                file_xml.Load(new StringReader(File.ReadAllText(intermidateFile)));
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

                File.Copy(NewDocToArchName, Path.Combine(StaticPathConfiguration.PathImplementFolder, Path.GetFileName(intermidateFile)), true);

                //File.AppendAllText("C:\\_test\\Arch_docs.log", "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

                new PgSql().ExecuteToDB(new string[7] { NameArray, EnvelopeID, DocumentID, PrDocumentName, PrDocumentNumber, DocCode, NewDocToArchName });

                if (Config.DeleteSourceFiles)
                {
                    File.Delete(intermidateFile);
                    File.Delete(NewDocToArchName);
                }
            });
        }
    }
}