using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XMLSigner.OutClass
{
    public class NormalizationXmlSign
    {
        /// <summary> </summary>
        /// <param name="pathToXmls"></param>
        public NormalizationXmlSign(string pathToXmlsFolder, int _MaxDegreeOfParallelism = -1)
        {
            string[] inplementFiles = Directory.GetFiles(pathToXmlsFolder);

            Parallel.ForEach(inplementFiles,
                new ParallelOptions { MaxDegreeOfParallelism = _MaxDegreeOfParallelism },
                inmplFile =>
            {
                /// XML Document Orig
                XmlDocument xmlDocOrigin = new XmlDocument();
                xmlDocOrigin.Load(new StringReader(File.ReadAllText(inmplFile)));
                XmlElement xmlRootOrigin = (XmlElement)xmlDocOrigin.GetElementsByTagName("Body")[0];

                /// XML Document Editing
                XmlDocument xmlDocEdit = new XmlDocument();
                xmlDocEdit.Load(new StringReader(File.ReadAllText(inmplFile)));
                XmlElement xmlRootEdit = (XmlElement)xmlDocEdit.GetElementsByTagName("Body")[0];

                FindElements(xmlRootEdit, xmlRootOrigin);

                xmlDocOrigin.Save(Path.Combine(StaticPath.PathSignedFolder, Path.GetFileName(inmplFile)));
            });
        }

        /// <summary> Сделать автопоиск по элементу Reference </summary>
        /// <param name="xmlEdit"></param>
        /// <param name="xmlOrig"></param>
        private void FindElements(XmlElement xmlEdit, XmlElement xmlOrig)
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
                    var strKeyHash = SignXMLGost.HashGostR3411_2012_256(swapKey);
                    ((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("DigestValue")[0].InnerText = strKeyHash;
                    ((XmlElement)objOrigNodes.Item(i)).GetElementsByTagName("DigestValue")[0].InnerText = strKeyHash;

                    /// Object hash
                    Normalization(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("Object", "*")[0]);
                    var swapObj = SwapAttributes(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("Object", "*")[0].OuterXml);
                    var strObjHash = SignXMLGost.HashGostR3411_2012_256(swapObj);
                    ((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("DigestValue")[1].InnerText = strObjHash;
                    ((XmlElement)objOrigNodes.Item(i)).GetElementsByTagName("DigestValue")[1].InnerText = strObjHash;

                    /// Sign Object
                    Normalization(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("SignedInfo", "*")[0]);
                    var swapSingedInfo = SwapAttributes(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("SignedInfo", "*")[0].OuterXml);
                    var strSignCms = SignXMLGost.SignCmsMessage(swapSingedInfo, SignXMLGost.Certificate);
                    ((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("SignatureValue")[0].InnerText = strSignCms;
                    ((XmlElement)objOrigNodes.Item(i)).GetElementsByTagName("SignatureValue")[0].InnerText = strSignCms;
                }
            }

            /// KeyInfo hash
            Normalization(xmlEdit.GetElementsByTagName("KeyInfo", "*")[0]);
            var swapBodyKey = SwapAttributes(((XmlElement)xmlEdit.GetElementsByTagName("KeyInfo", "*")[0]).OuterXml);
            var strBodyKeyHash = SignXMLGost.HashGostR3411_2012_256(swapBodyKey);
            ((XmlElement)xmlEdit.GetElementsByTagName("DigestValue")[0]).InnerText = strBodyKeyHash;
            ((XmlElement)xmlOrig.GetElementsByTagName("DigestValue")[0]).InnerText = strBodyKeyHash;

            /// Object hash
            Normalization(xmlEdit.GetElementsByTagName("Object", "*")[0]);
            var swapBodyObj = SwapAttributes(((XmlElement)xmlEdit.GetElementsByTagName("Object", "*")[0]).OuterXml);
            var strBodyObjHash = SignXMLGost.HashGostR3411_2012_256(swapBodyObj);
            ((XmlElement)xmlEdit.GetElementsByTagName("DigestValue")[1]).InnerText = strBodyObjHash;
            ((XmlElement)xmlOrig.GetElementsByTagName("DigestValue")[1]).InnerText = strBodyObjHash;

            /// Sign Object
            Normalization(xmlEdit.GetElementsByTagName("SignedInfo", "*")[0]);
            var swapBodySingedInfo = SwapAttributes(((XmlElement)xmlEdit.GetElementsByTagName("SignedInfo", "*")[0]).OuterXml);
            var strBodySignCms = SignXMLGost.SignCmsMessage(swapBodySingedInfo, SignXMLGost.Certificate);
            ((XmlElement)xmlEdit.GetElementsByTagName("SignatureValue")[0]).InnerText = strBodySignCms;
            ((XmlElement)xmlOrig.GetElementsByTagName("SignatureValue")[0]).InnerText = strBodySignCms;
        }

        /// <summary> Нормализация Xml документа</summary>
        /// <param name="xmlNode"></param>
        /// <param name="prefix"></param>
        /// <param name="rootNode"></param>
        public void Normalization(XmlNode xmlNode, string prefix = "n1")
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
        private string SwapAttributes(string OuterXml, bool disableFormating = true)
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
    }
}
