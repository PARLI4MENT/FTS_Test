using SQLNs;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using XmlFTS.OutClass;
using XMLSigner.OutClass;

namespace XmlFTS
{
    public static class TemplatingXml
    {

        /// <summary> Извлекает уникальный ID (DocumentId) XML-документа </summary>
        /// <param name="pathToXML">Абсолютный путь к XML-документу</param>
        /// <returns>Возвращает уникальный ID в виде строки</returns>
        public static string ExtractId(string pathToXML)
        {
            if (String.IsNullOrEmpty(pathToXML))
            {
                Debug.WriteLine("IS NULL OR EMPTY");
                return null;
            }

            using (StreamReader sr = new StreamReader(pathToXML, true))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(sr);
                XmlElement docId = (XmlElement)xmlDoc.DocumentElement.GetElementsByTagName("ct:DocumentID")[0];
                return docId.InnerText;
            }
        }

        /// <summary> Извлекает уникальный ID (DocumentId) XML-документа </summary>
        /// <param name="pathToXML">Абсолютный путь к XML-документу</param>
        /// <param name="id">Возвращает уникальный ID в виде строки</param>
        public static void ExtractId(string pathToXML, out string id)
        {
            if (String.IsNullOrEmpty(pathToXML))
            {
                Debug.WriteLine("IS NULL OR EMPTY");
                id = null;
                return;
            }

            using (StreamReader sr = new StreamReader(pathToXML, true))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(sr);
                XmlElement docId = (XmlElement)xmlDoc.DocumentElement.GetElementsByTagName("ct:DocumentID")[0];
                id = docId.InnerText;
            }
        }

        /// <summary> Извлечение данные из промежуточных XML-файлов, вставка в шаблонный XML и сохранение в папку (по-умолчанию implementFiles ) </summary>
        /// <param name="extractedFile"> Путь к XML-файлу</param>
        /// <returns></returns>
        public static void TemplatingLinear(string extractedFile, ref X509Certificate2 cert, string MchdId = "", string MchdINN = "")
        {
            string NameArray = (string)Path.GetFileName(extractedFile).Split('.')[0];
            var file_xml = new XmlDocument();
            var doc_to_arch = new XmlDocument();

            string PrDocumentName = "", PrDocumentNumber = "", PrDocumentDate = "", DocCode = "", DocName = "";

            string NewDocToArchName = Path.Combine(Path.GetDirectoryName(extractedFile), Path.GetFileName(extractedFile));
            File.Copy(StaticPathConfiguration.TemplateXML, NewDocToArchName, true);

            file_xml.Load(new StringReader(File.ReadAllText(extractedFile)));
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

            doc_to_arch.Load(extractedFile);

            var temp_node = doc_to_arch.ImportNode(file_xml.DocumentElement, true);
            doc_to_arch.GetElementsByTagName("Object")[1].AppendChild(temp_node);

            string EnvelopeID = Guid.NewGuid().ToString().ToUpper();
            string DocumentID = Guid.NewGuid().ToString().ToUpper();

            doc_to_arch.GetElementsByTagName("EnvelopeID", "*")[0].InnerText = EnvelopeID;
            doc_to_arch.GetElementsByTagName("roi:SenderInformation")[0].InnerText = "smtp://eps.customs.ru/nts102773904951145";
            doc_to_arch.GetElementsByTagName("roi:PreparationDateTime")[0].InnerText = DateTime.Now.ToString("s") + DateTime.Now.ToString("zzz");
            doc_to_arch.GetElementsByTagName("ParticipantID")[0].InnerText = "102773904741735";
            doc_to_arch.GetElementsByTagName("CustomsCode")[0].InnerText = "10000000";
            
            doc_to_arch.GetElementsByTagName("X509Certificate")[0].InnerText = Convert.ToBase64String(cert.RawData);

            doc_to_arch.GetElementsByTagName("ct:DocumentID")[0].InnerText = Guid.NewGuid().ToString().ToUpper();
            doc_to_arch.GetElementsByTagName("ct:ArchDeclID")[0].InnerText = "ArchDeclID_TEMP";
            doc_to_arch.GetElementsByTagName("ct:ArchID")[0].InnerText = "ArchID_TEMP";
            doc_to_arch.GetElementsByTagName("DocumentID", "*")[1].InnerText = DocumentID;
            doc_to_arch.GetElementsByTagName("DocCode", "*")[0].InnerText = DocCode;
            doc_to_arch.GetElementsByTagName("X509Certificate", "*")[0].InnerText = Convert.ToBase64String(cert.RawData);

            ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentName", "*")[0].InnerText = PrDocumentName;

            if (string.IsNullOrEmpty(PrDocumentNumber))
                doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0].RemoveChild(((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentNumber", "*")[0]);
            else
                ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentNumber", "*")[0].InnerText = PrDocumentNumber;

            if (string.IsNullOrEmpty(PrDocumentDate))
                doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0].RemoveChild(((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentDate", "*")[0]);
            else
                ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentDate", "*")[0].InnerText = PrDocumentDate;

            /// Если использутся МЧД
            if (!string.IsNullOrEmpty(MchdId) && !string.IsNullOrEmpty(MchdINN))
            {
                var KeyInfos = doc_to_arch.GetElementsByTagName("KeyInfo", "*");
                foreach (XmlElement KeyInfo in KeyInfos)
                {
                    var xmlMCDid = doc_to_arch.CreateElement("MCDId", KeyInfo.NamespaceURI);
                    KeyInfo.AppendChild(xmlMCDid).InnerText = MchdId.ToUpper();

                    var xmlINNPrincipal = doc_to_arch.CreateElement("INNPrincipal", KeyInfo.NamespaceURI);
                    KeyInfo.AppendChild(xmlINNPrincipal).InnerText = MchdINN.ToUpper();
                }
            }
            doc_to_arch.Save(NewDocToArchName);
            string templatePath = Path.Combine(StaticPathConfiguration.PathTemplatedFolder, Path.GetFileName(extractedFile));
            File.Copy(NewDocToArchName, templatePath, true);

            //File.AppendAllText("C:\\_test\\Arch_docs.log", "New TEST;START;END CASE;PREP XML;SING XML;INSERT;");

            // Query to PostgresSql DB
            new PgSql().ExecuteToDB(new string[7] { NameArray, EnvelopeID, DocumentID, PrDocumentName, PrDocumentNumber, DocCode, NewDocToArchName });
            
            /// Нормализация и подписание
            NormalizationXmlSign.NormalizationXml(NewDocToArchName, ref cert);

            if (Config.DeleteSourceFiles)
                File.Delete(templatePath);
        }


        /// <summary>Создание XML-файла запроса на создание архива</summary>
        /// <param name="MchdId">МЧД в виде строки</param>
        /// <param name="INN">ИНН в виде строки</param>
        /// <param name="cert">Объект сертификата класса X509Certificate2</param>
        /// <param name="ArchiveName"></param>
        public static void CreateArchive(string MchdId, string INN, X509Certificate2 cert, [Optional] string ArchiveName)
        {
            string pathToTemplate = "C:\\Test\\create_arch.xml";
            string newArchivePath = Path.Combine("C:\\Test\\CreateArchive", Path.GetFileName(pathToTemplate));
            File.Copy(pathToTemplate, newArchivePath, true);

            /// XML
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(new StringReader(File.ReadAllText(newArchivePath)));

            string EnvelopeID = Guid.NewGuid().ToString().ToUpper();
            string DocumentID = Guid.NewGuid().ToString().ToUpper();

            xmlDoc.GetElementsByTagName("EnvelopeID", "*")[0].InnerText = EnvelopeID;
            xmlDoc.GetElementsByTagName("roi:SenderInformation")[0].InnerText = "smtp://eps.customs.ru/nts102773904741735";
            xmlDoc.GetElementsByTagName("roi:PreparationDateTime")[0].InnerText = DateTime.Now.ToString("s") + DateTime.Now.ToString("zzz");
            xmlDoc.GetElementsByTagName("ParticipantID")[0].InnerText = "102773904741735";
            xmlDoc.GetElementsByTagName("CustomsCode")[0].InnerText = "10000000";

            xmlDoc.GetElementsByTagName("X509Certificate")[0].InnerText = Convert.ToBase64String(cert.RawData);
            xmlDoc.GetElementsByTagName("ct:DocumentID")[0].InnerText = Guid.NewGuid().ToString().ToUpper();

            if (String.IsNullOrEmpty(ArchiveName))
                ArchiveName = DateTime.Now.ToString("H-mm-ss_dd.MM.yyyy");

            xmlDoc.GetElementsByTagName("ArchiveName")[0].InnerText = ArchiveName;
            xmlDoc.GetElementsByTagName("ArchDeclID")[0].InnerText = "102773904741735";

            var KeyInfos = xmlDoc.GetElementsByTagName("KeyInfo", "*");
            foreach (XmlElement KeyInfo in KeyInfos)
            {
                var xmlMCDid = xmlDoc.CreateElement("MCDId", KeyInfo.NamespaceURI);
                KeyInfo.AppendChild(xmlMCDid).InnerText = MchdId.ToUpper();

                var xmlINNPrincipal = xmlDoc.CreateElement("INNPrincipal", KeyInfo.NamespaceURI);
                KeyInfo.AppendChild(xmlINNPrincipal).InnerText = INN.ToUpper();
            }

            Console.WriteLine();

            xmlDoc.Save(newArchivePath);
            NormalizationXmlSign.NormalizationXmlForArchive(newArchivePath, ref cert);
            Console.WriteLine();
        }
    }
}