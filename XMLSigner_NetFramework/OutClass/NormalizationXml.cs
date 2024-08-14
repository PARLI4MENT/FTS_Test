using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System;
using System.Collections.Generic;

namespace XMLSigner.OutClass
{
    public class NormalizationXml
    {
        public NormalizationXml(string pathToXml)
        {
            /// XML Document Orig
            XmlDocument xmlDocOrigin = new XmlDocument();
            xmlDocOrigin.Load(new StringReader(File.ReadAllText(pathToXml)));
            XmlElement xmlRootOrigin = (XmlElement)xmlDocOrigin.GetElementsByTagName("Body")[0];

            /// XML Document Editing
            XmlDocument xmlDocEdit = new XmlDocument();
            xmlDocEdit.Load(new StringReader(File.ReadAllText(pathToXml)));
            XmlElement xmlRootEdit = (XmlElement)xmlDocEdit.GetElementsByTagName("Body")[0];

            FindReference(xmlRootEdit, xmlRootOrigin);

            Console.WriteLine();
            xmlDocOrigin.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"sign_{Path.GetFileName(pathToXml)}"));
            Console.WriteLine();
        }

        /// <summary> Сделать автопоиск по элементу Reference </summary>
        /// <param name="xmlEdit"></param>
        /// <param name="xmlOrig"></param>
        private void FindReference(XmlElement xmlEdit, XmlElement xmlOrig)
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
                    Console.WriteLine();

                    /// Object hash
                    Normalization(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("Object", "*")[0]);
                    var swapObj = SwapAttributes(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("Object", "*")[0].OuterXml);
                    var strObjHash = SignXMLGost.HashGostR3411_2012_256(swapObj);
                    ((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("DigestValue")[1].InnerText = strObjHash;
                    ((XmlElement)objOrigNodes.Item(i)).GetElementsByTagName("DigestValue")[1].InnerText = strObjHash;
                    Console.WriteLine();

                    /// Sign Object
                    Normalization(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("SignedInfo", "*")[0]);
                    var swapSingedInfo = SwapAttributes(((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("SignedInfo", "*")[0].OuterXml);
                    var strSignCms = SignXMLGost.SignCmsMessage(swapSingedInfo, SignXMLGost.Certificate);
                    ((XmlElement)objEditNodes.Item(i)).GetElementsByTagName("SignatureValue")[0].InnerText = strSignCms;
                    ((XmlElement)objOrigNodes.Item(i)).GetElementsByTagName("SignatureValue")[0].InnerText = strSignCms;
                    Console.WriteLine();
                }
            }
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
