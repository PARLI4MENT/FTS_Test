using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XmlFTS.OutClass;
using System.Security.Cryptography.X509Certificates;

namespace XMLSigner.OutClass
{
    public static class NormalizationXmlSign
    {
        /// <summary></summary>
        /// <param name="pathToXmls"></param>
        public static void NormalizationXml(string pathToXml, ref X509Certificate2 cert)
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

        /// <summary> Сделать автопоиск по элементу Reference </summary>
        /// <param name="xmlEdit"></param>
        /// <param name="xmlOrig"></param>
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

        /// <summary> Нормализация XmlNode </summary>
        /// <param name="xmlNode"></param>
        /// <param name="prefix"></param>
        /// <param name="rootNode"></param>
        /// <remarks>Из VBasic (не используется)</remarks>
        private static void Normalization(XmlNode xmlNode, string prefix = "n1")
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

        /// <summary> "Открывает" автозакрытые элементы XML-документа </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static XmlNode OpenAutoClosed(XmlNode node)
        {
            if (String.IsNullOrEmpty(node.InnerText))
            {

            }
            return null;
        }

        /// <summary> Возвращает xml-элементы включая входящий элемент, с его дочерними элементами в виде древа </summary>
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
