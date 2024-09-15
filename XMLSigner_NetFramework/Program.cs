#define TEST
///5.23.0/3.4.16

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using XmlFTS.OutClass;

namespace XMLSigner
{
    public static class Program
    {
        static string PathToTemplate = "C:\\_2\\template.xml";

        /// <summary> Это нужно будет удалить </summary>
        static string MchdId = "e7d94ee1-33d4-4b95-a27d-07896fdc00e0";
        static string MchdINN = "250908790897";
        static X509Certificate2 cert = SignXmlGost.FindGostCurrentCertificate("01DA FCE9 BC8E 41B0 0008 7F5E 381D 0002");
        
        static void Main(string[] args)
        {
            OpenExternalFile.Open(PathToTemplate);

            Console.WriteLine();

            Console.Write("\nPress any key...");
            Console.ReadKey();
            Console.ReadKey();
        }

        private static void ProcessStart()
        {
            var sw = new Stopwatch();
            sw.Start();
            int SummaryFiles = 0;
            Console.WriteLine("Start...");

            var rawSrcFolders = Directory.GetDirectories("C:\\Dekl\\SEND DATA");
            foreach (var rawSrcFolder in rawSrcFolders)
            {
                /// #1 Extraction ZIP
                ArchiveWorker.ExtractZipArchive(Directory.GetFiles(rawSrcFolder, "*.zip")[0]);

                /// #2 Rename Copy
                string[] xmlFiles = Directory.GetFiles(rawSrcFolder, "*.xml");
                RenameMoveFileOnly(xmlFiles);
            }

            Console.WriteLine("Sort start");

            ///// #3 Sort
            string[] notSortedFiles = Directory.GetFiles("C:\\_2\\ExtractionFiles", "*.xml");   
            SortXml(notSortedFiles);

            SummaryFiles += Directory.GetFiles("C:\\_2\\ExtractionFiles", "*.xml").Count();

            sw.Stop();
            Console.WriteLine();
            Console.WriteLine($"General => {SummaryFiles} count || {sw.ElapsedMilliseconds / 1000} sec.");
            //Console.WriteLine($"AVG => {SummaryFiles / (sw.ElapsedMilliseconds / 1000)} sec.");

            Console.WriteLine();
            Console.Write("Press any key...");
            Console.ReadKey();
        }

        /// <summary>Переименование и перемещение</summary>
        /// <param name="pathRawFile">Путь к файлу</param>
        public static string RenameMoveFileOnly(string pathRawFile)
        {
            string code = Path.GetFileName(Path.GetDirectoryName(pathRawFile));

            if (!Directory.Exists(Path.Combine("C:\\_2\\ExtractionFiles")))
                Directory.CreateDirectory(Path.Combine("C:\\_2\\ExtractionFiles"));

            if (File.Exists(pathRawFile))
            {
                string tmpPathCombine = Path.Combine("C:\\_2\\ExtractionFiles", string.Concat(code, ".", Path.GetFileName(pathRawFile)));
                if (!File.Exists(tmpPathCombine))
                {
                    File.Copy(pathRawFile, tmpPathCombine, true);
                    return tmpPathCombine;
                }
            }
            return null;
        }

        /// <summary> Переименование и перемещение </summary>
        /// <remarks>НЕ ИСПОЛЬЗОВАТЬ</remarks>
        /// <param name="xmlFiles">Массив строк путей к файлам</param>
        /// <returns></returns>
        private static string RenameMoveFileOnly(string[] xmlFiles)
        {
            foreach (var xmlFile in xmlFiles)
            {
                string code = Path.GetFileName(Path.GetDirectoryName(xmlFile));

                if (!Directory.Exists(Path.Combine("C:\\_2\\ExtractionFiles")))
                    Directory.CreateDirectory(Path.Combine("C:\\_2\\ExtractionFiles"));

                if (File.Exists(xmlFile))
                {
                    string tmpPathCombine = Path.Combine("C:\\_2\\ExtractionFiles", string.Concat(code, ".", Path.GetFileName(xmlFile)));
                    if (!File.Exists(tmpPathCombine))
                    {
                        File.Copy(xmlFile, tmpPathCombine, true);
                        return tmpPathCombine;
                    }
                }
            }
            return null;
        }

        public static void SortXml(string[] xmlFiles)
        {

            Parallel.ForEach
                (xmlFiles,
                new ParallelOptions { MaxDegreeOfParallelism = 1 },
                xmlFile =>
                {
                    if (File.Exists(xmlFile))
                    {
                        string tmpFilePath;

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(new StringReader(File.ReadAllText(xmlFile)));

                        switch (xmlDoc.DocumentElement.GetAttribute("DocumentModeID"))
                        {
                            /// ПТД ExpressCargoDeclaration
                            case "1006275E":
                                {
                                    tmpFilePath = Path.Combine("C:\\_2\\Sorted\\ptd", Path.GetFileName(xmlFile));
                                    //if (!File.Exists(tmpFilePath))
                                    //{
                                        File.Copy(xmlFile, tmpFilePath, true);
                                        
                                        // Шаблонизация
                                        var tmp = ImplementLinear(tmpFilePath, true);

                                        /// выбрать серификат Конкретного человека

                                        /// Нормализация и подписание
                                        NormalizationXml(tmp, cert);
                                    //}
                                }
                                break;

                            /// В архив Остальное
                            default:
                                {
                                    tmpFilePath = Path.Combine("C:\\_2\\Sorted\\toArchive", Path.GetFileName(xmlFile));
                                    //if (!File.Exists(tmpFilePath))
                                    //{
                                        File.Copy(xmlFile, tmpFilePath, true);

                                        /// Шаблонизация
                                        var tmp = ImplementLinear(tmpFilePath, true);

                                        /// выбрать серификат (Компании) ///Пока индивидуальный

                                        /// Нормализация и подписание
                                        NormalizationXml(tmp, cert);
                                    //}
                                }
                                break;
                        }
                    }
                });
        }

        public static string ImplementLinear(string intermidateFile, bool isMCHD = false)
        {
            string NameArray = (string)Path.GetFileName(intermidateFile).Split('.')[0];
            var file_xml = new XmlDocument();
            var doc_to_arch = new XmlDocument();

            string PrDocumentName = "", PrDocumentNumber = "", PrDocumentDate = "", DocCode = "", DocName = "";

            string NewDocToArchName = Path.Combine(Path.GetDirectoryName(intermidateFile), Path.GetFileName(intermidateFile));
            File.Copy(PathToTemplate, NewDocToArchName, true);

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

            doc_to_arch.GetElementsByTagName("EnvelopeID", "*")[0].InnerText = Guid.NewGuid().ToString().ToUpper();
            //var envelopIds = doc_to_arch.GetElementsByTagName("EnvelopeID", "*");
            //foreach (XmlNode envelopId in envelopIds)
            //    envelopId.InnerText = EnvelopeID;
            //doc_to_arch.GetElementsByTagName("roi:SenderInformation")[0].InnerText = "smpt://eps.customs.ru/nts102773904741735";
            doc_to_arch.GetElementsByTagName("roi:SenderInformation")[0].InnerText = "smtp://eps.customs.ru/nts102773904951145";
            //var senderInfos = doc_to_arch.GetElementsByTagName("roi:SenderInformation");
            //foreach (XmlNode senderInfo in senderInfos)
            //    senderInfo.InnerText = "smpt://eps.customs.ru/nts102773904741735";
            doc_to_arch.GetElementsByTagName("roi:PreparationDateTime")[0].InnerText = DateTime.Now.ToString("s") + DateTime.Now.ToString("zzz");
            //var preparationDataTimes = doc_to_arch.GetElementsByTagName("roi:PreparationDateTime");
            //foreach (XmlNode preparationDataTime in preparationDataTimes)
            //    preparationDataTime.InnerText = DateTime.Now.ToString("s") + DateTime.Now.ToString("zzz");
            doc_to_arch.GetElementsByTagName("ParticipantID")[0].InnerText = "102773904741735";
            //var participantIds = doc_to_arch.GetElementsByTagName("ParticipantID");
            //foreach (XmlNode participantId in participantIds)
            //    participantId.InnerText = "102773904741735";
            doc_to_arch.GetElementsByTagName("CustomsCode")[0].InnerText = "10000000";
            //var customsCodes = doc_to_arch.GetElementsByTagName("CustomsCode");
            //foreach (XmlNode customsCode in customsCodes)
            //    customsCode.InnerText = "10000000";
            //doc_to_arch.GetElementsByTagName("X509Certificate")[0].InnerText = Convert.ToBase64String(cert.RawData);
            //var x509Certs = doc_to_arch.GetElementsByTagName("X509Certificate");
            //foreach (XmlNode x509Cert in x509Certs)
            //    x509Cert.InnerText = Convert.ToBase64String(cert.RawData);
            doc_to_arch.GetElementsByTagName("ct:DocumentID")[0].InnerText = (Guid.NewGuid().ToString()).ToUpper();
            //var ctDocIds = doc_to_arch.GetElementsByTagName("ct:DocumentID");
            //foreach (XmlNode ctDocId in ctDocIds)
            //    ctDocId.InnerText = (Guid.NewGuid().ToString()).ToUpper();
            doc_to_arch.GetElementsByTagName("ct:ArchDeclID")[0].InnerText = "ArchDeclID_TEMP";
            //var archDeclIds = doc_to_arch.GetElementsByTagName("ct:ArchDeclID");
            //foreach (XmlNode archDeclId in archDeclIds)
            //    archDeclId.InnerText = "ArchDeclID_TEMP";
            doc_to_arch.GetElementsByTagName("ct:ArchID")[0].InnerText = "ArchID_TEMP";
            //var archIds = doc_to_arch.GetElementsByTagName("ct:ArchID");
            //foreach (XmlNode archId in archIds)
            //    archId.InnerText = "ArchID_TEMP";
            doc_to_arch.GetElementsByTagName("DocumentID", "*")[1].InnerText = Guid.NewGuid().ToString().ToUpper();
            doc_to_arch.GetElementsByTagName("DocCode", "*")[0].InnerText = DocCode;
            //var docCodes = doc_to_arch.GetElementsByTagName("DocCode", "*");
            //foreach (XmlNode docCode in docCodes)
            //    docCode.InnerText = DocCode;
            doc_to_arch.GetElementsByTagName("X509Certificate", "*")[0].InnerText = Convert.ToBase64String(cert.RawData);
            //var x509CertTemps = doc_to_arch.GetElementsByTagName("X509Certificate", "*");
            //foreach (XmlNode x509CertTemp in x509CertTemps)
            //    x509CertTemp.InnerText = Convert.ToBase64String(cert.RawData);

            ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentName", "*")[0].InnerText = PrDocumentName;

            if (string.IsNullOrEmpty(PrDocumentNumber))
                doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0].RemoveChild(((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentNumber", "*")[0]);
            else
                ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentNumber", "*")[0].InnerText = PrDocumentNumber;

            if (string.IsNullOrEmpty(PrDocumentDate))
                doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0].RemoveChild(((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentDate", "*")[0]);
            else
                ((XmlElement)doc_to_arch.GetElementsByTagName("DocBaseInfo", "*")[0]).GetElementsByTagName("PrDocumentDate", "*")[0].InnerText = PrDocumentDate;

            if (isMCHD)
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

            //var implXml = Path.Combine(Path.GetDirectoryName(intermidateFile), Path.GetFileName(intermidateFile));
            ////// ERROR
            //File.Copy(NewDocToArchName, implXml, true);

            return NewDocToArchName;
        }

        public static void NormalizationXml(string pathToXml, X509Certificate2 cert)
        {
            /// XML Document Orig
            XmlDocument xmlDocOrigin = new XmlDocument();
            xmlDocOrigin.Load(new StringReader(File.ReadAllText(pathToXml)));
            XmlElement xmlRootOrigin = (XmlElement)xmlDocOrigin.GetElementsByTagName("Body")[0];

            /// XML Document Editing
            XmlDocument xmlDocEdit = new XmlDocument();
            xmlDocEdit.Load(new StringReader(File.ReadAllText(pathToXml)));
            XmlElement xmlRootEdit = (XmlElement)xmlDocEdit.GetElementsByTagName("Body")[0];

            FindElements(xmlRootEdit, xmlRootOrigin, ref cert);

            var normXml = Path.Combine("C:\\_2\\SignedFiles", Path.GetFileName(pathToXml));

            xmlDocOrigin.Save(normXml);

            /// Save without formating
            var xDocument = XDocument.Load(normXml);
            xDocument.Save(normXml, SaveOptions.DisableFormatting);
        }

        public static XmlNode OpenAutoClosed(XmlNode node)
        {
            if (String.IsNullOrEmpty(node.InnerText))
            {

            }


            return null;
        }

        public static void FindElements(XmlElement xmlEdit, XmlElement xmlOrig, ref X509Certificate2 cert)
        {
            var objEditNodes = xmlEdit.GetElementsByTagName("Object");
            var objOrigNodes = xmlOrig.GetElementsByTagName("Object");

            for (int i = objEditNodes.Count - 1; i >= 0; i--)
            {
                if (objEditNodes.Item(i).HasChildNodes)
                {
                    /// KeyInfo hash
                    Normalization(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("KeyInfo", "*")[0]);
                    var swapKey = SwapAttributes(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("KeyInfo", "*")[0].OuterXml);
                    var strKeyHash = SignXmlGost.HashGostR3411_2012_256(swapKey);
                    ((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("DigestValue")[0].InnerText = strKeyHash;
                    ((XmlElement)objOrigNodes.Item(i)).GetElementsByTagName("DigestValue")[0].InnerText = strKeyHash;

                    /// Object hash
                    Normalization(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("Object", "*")[0]);
                    var swapObj = SwapAttributes(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("Object", "*")[0].OuterXml);
                    var strObjHash = SignXmlGost.HashGostR3411_2012_256(swapObj);
                    ((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("DigestValue")[1].InnerText = strObjHash;
                    ((XmlElement)objOrigNodes.Item(i)).GetElementsByTagName("DigestValue")[1].InnerText = strObjHash;

                    /// Sign Object
                    Normalization(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("SignedInfo", "*")[0]);
                    var swapSingedInfo = SwapAttributes(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("SignedInfo", "*")[0].OuterXml);
                    var strSignCms = SignXmlGost.SignCmsMessage(swapSingedInfo, cert);
                    ((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("SignatureValue")[0].InnerText = strSignCms;
                    ((XmlElement)objOrigNodes.Item(i)).GetElementsByTagName("SignatureValue")[0].InnerText = strSignCms;
                }
            }

            /// KeyInfo hash
            Normalization(xmlEdit.GetElementsByTagName("KeyInfo", "*")[0]);
            var swapBodyKey = SwapAttributes(((XmlElement)xmlEdit.GetElementsByTagName("KeyInfo", "*")[0]).OuterXml);
            var strBodyKeyHash = SignXmlGost.HashGostR3411_2012_256(swapBodyKey);
            ((XmlElement)xmlEdit.GetElementsByTagName("DigestValue")[0]).InnerText = strBodyKeyHash;
            ((XmlElement)xmlOrig.GetElementsByTagName("DigestValue")[0]).InnerText = strBodyKeyHash;

            /// Object hash
            Normalization(xmlEdit.GetElementsByTagName("Object", "*")[0]);
            var swapBodyObj = SwapAttributes(((XmlElement)xmlEdit.GetElementsByTagName("Object", "*")[0]).OuterXml);
            var strBodyObjHash = SignXmlGost.HashGostR3411_2012_256(swapBodyObj);
            ((XmlElement)xmlEdit.GetElementsByTagName("DigestValue")[1]).InnerText = strBodyObjHash;
            ((XmlElement)xmlOrig.GetElementsByTagName("DigestValue")[1]).InnerText = strBodyObjHash;

            /// Sign Object
            Normalization(xmlEdit.GetElementsByTagName("SignedInfo", "*")[0]);
            var swapBodySingedInfo = SwapAttributes(((XmlElement)xmlEdit.GetElementsByTagName("SignedInfo", "*")[0]).OuterXml);
            var strBodySignCms = SignXmlGost.SignCmsMessage(swapBodySingedInfo, cert);
            ((XmlElement)xmlEdit.GetElementsByTagName("SignatureValue")[0]).InnerText = strBodySignCms;
            ((XmlElement)xmlOrig.GetElementsByTagName("SignatureValue")[0]).InnerText = strBodySignCms;
        }

        /// <summary> Нормализация Xml документа</summary>
        /// <param name="xmlNode"></param>
        /// <param name="prefix"></param>
        /// <param name="rootNode"></param>
        public static void Normalization(XmlNode xmlNode, string prefix = "n1")
        {
            if (xmlNode.GetType().Equals(typeof(XmlElement)))
            {
                var elem = (XmlElement)xmlNode;

                if (elem.HasChildNodes)
                    foreach (var node in elem.ChildNodes)
                        if (node.GetType().Equals(typeof(XmlElement)))
                            Normalization((XmlElement)node);

                elem.Prefix = "n1";
                elem.IsEmpty = false;

                if (elem.HasAttributes)
                {
                    for (int i = 0; i < elem.Attributes.Count;)
                    {
                        if (elem.Attributes[i].Name.Contains("xmlns"))
                        {
                            elem.RemoveAttributeAt(i);
                            continue;
                        }
                        i++;
                    }
                }
            }
        }

        /// <summary> Смена xmlns и аттрибута местами</summary>
        /// <param name="OuterXml"></param>
        /// <param name="disableFormating"></param>
        /// <returns></returns>
        private static string SwapAttributes(string OuterXml, bool disableFormating = true)
        {
            XDocument xDoc = XDocument.Parse(OuterXml);
            XElement xElement = xDoc.Root;

            foreach (var elem in xElement.DescendantNodesAndSelf())
            {
                if (elem is XElement)
                {
                    if (((XElement)elem).HasAttributes && ((XElement)elem).Attributes().Count() > 1)
                    {
                        if (!((XElement)elem).Attributes().First().Name.ToString().Contains("xmlns:n1"))
                        {
                            var fAttr = ((XElement)elem).Attributes().First();
                            var sAttr = ((XElement)elem).Attributes().Last();

                            ((XElement)elem).Attributes().Remove();

                            ((XElement)elem).SetAttributeValue(sAttr.Name, sAttr.Value);
                            ((XElement)elem).SetAttributeValue(fAttr.Name, fAttr.Value);
                        }
                    }
                }
            }

            if (disableFormating)
                return xElement.ToString(SaveOptions.DisableFormatting);
            return xElement.ToString();
        }

        /// <summary> Возвращает xml-элементы включая входящий элемент, с его дочерними элементами в виде древа </summary>
        /// <remarks> </remarks>
        /// <param name="element"></param>
        public static void GetTree(XmlElement element)
        {
            if (element.GetType().Equals(typeof(XmlElement)))
            {
                foreach (var node in element.ChildNodes)
                {
                    if (node.GetType().Equals(typeof(XmlElement)))
                    {
                        var elem = (XmlElement)node;
                        Console.WriteLine($"\t{elem.Name}");
                        if (elem.InnerText == "" && elem.InnerText == null)
                            GetTree(elem);
                    }
                }
            }
        }
    }
}