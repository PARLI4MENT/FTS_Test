
using System.Collections.Generic;
using System.Xml.Serialization;
using System;

//namespace EnvelopXMLSerialize
namespace EnvelopXMLSerialize
{
    [XmlRoot(ElementName = "RoutingInf", Namespace = "urn:customs.ru:Envelope:RoutingInf:1.0")]
    public class RoutingInf
    {
        [XmlElement(ElementName = "EnvelopeID", Namespace = "urn:customs.ru:Envelope:RoutingInf:1.0")]
        public string EnvelopeID;

        [XmlElement(ElementName = "SenderInformation", Namespace = "urn:customs.ru:Envelope:RoutingInf:1.0")]
        public string SenderInformation;

        [XmlElement(ElementName = "ReceiverInformation", Namespace = "urn:customs.ru:Envelope:RoutingInf:1.0")]
        public string ReceiverInformation;

        [XmlElement(ElementName = "PreparationDateTime", Namespace = "urn:customs.ru:Envelope:RoutingInf:1.0")]
        public DateTime PreparationDateTime;
    }

    [XmlRoot(ElementName = "ApplicationInf", Namespace = "urn:customs.ru:Envelope:ApplicationInf:1.0")]
    public class ApplicationInf
    {
        [XmlElement(ElementName = "SoftVersion", Namespace = "urn:customs.ru:Envelope:ApplicationInf:1.0")]
        public string SoftVersion;
    }

    [XmlRoot(ElementName = "ReceiverCustoms", Namespace = "urn:customs.ru:Envelope:EDHeader:2.0")]
    public class ReceiverCustoms
    {
        [XmlElement(ElementName = "CustomsCode", Namespace = "urn:customs.ru:Envelope:EDHeader:2.0")]
        public int CustomsCode;

        [XmlElement(ElementName = "ExchType", Namespace = "urn:customs.ru:Envelope:EDHeader:2.0")]
        public int ExchType;
    }

    [XmlRoot(ElementName = "EDHeader", Namespace = "urn:customs.ru:Envelope:EDHeader:2.0")]
    public class EDHeader
    {
        [XmlElement(ElementName = "MessageType", Namespace = "urn:customs.ru:Envelope:EDHeader:2.0")]
        public string MessageType;

        [XmlElement(ElementName = "ParticipantID", Namespace = "urn:customs.ru:Envelope:EDHeader:2.0")]
        public string ParticipantID;

        [XmlElement(ElementName = "ReceiverCustoms", Namespace = "urn:customs.ru:Envelope:EDHeader:2.0")]
        public ReceiverCustoms ReceiverCustoms;

        [XmlAttribute(AttributeName = "xmlns", Namespace = "")]
        public string Xmlns;

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi;

        [XmlText]
        public string Text;
    }

    [XmlRoot(ElementName = "Header", Namespace = "http://www.w3.org/2001/06/soap-envelope")]
    public class Header
    {
        [XmlElement(ElementName = "RoutingInf", Namespace = "urn:customs.ru:Envelope:RoutingInf:1.0")]
        public RoutingInf RoutingInf;

        [XmlElement(ElementName = "ApplicationInf", Namespace = "urn:customs.ru:Envelope:ApplicationInf:1.0")]
        public ApplicationInf ApplicationInf;

        [XmlElement(ElementName = "EDHeader", Namespace = "urn:customs.ru:Envelope:EDHeader:2.0")]
        public EDHeader EDHeader;
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm", Namespace = "")]
        public string Algorithm;
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm", Namespace = "")]
        public string Algorithm;
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm", Namespace = "")]
        public string Algorithm;
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform;
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm", Namespace = "")]
        public string Algorithm;
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms;

        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod;

        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue;

        [XmlAttribute(AttributeName = "URI", Namespace = "")]
        public string URI;

        [XmlText]
        public string Text;
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod;

        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod;

        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public List<Reference> Reference;
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate;
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data;

        [XmlAttribute(AttributeName = "Id", Namespace = "")]
        public string Id;

        [XmlText]
        public string Text;
    }

    [XmlRoot(ElementName = "Object", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Object
    {
        [XmlAttribute(AttributeName = "Id", Namespace = "")]
        public string Id;

        [XmlElement(ElementName = "ArchAddDocRequest", Namespace = "urn:customs.ru:Information:EArchDocuments:ArchAddDocRequest:5.13.1")]
        public ArchAddDocRequest ArchAddDocRequest;

        [XmlText]
        public string Text;

        [XmlElement(ElementName = "Envelope", Namespace = "http://www.w3.org/2001/06/soap-envelope")]
        public Envelope Envelope;
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo;

        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue;

        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo;

        [XmlElement(ElementName = "Object", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Object Object;

        [XmlAttribute(AttributeName = "xmlns", Namespace = "")]
        public string Xmlns;

        [XmlText]
        public string Text;
    }

    [XmlRoot(ElementName = "ArchDoc", Namespace = "urn:customs.ru:Information:EArchDocuments:ArchAddDocRequest:5.13.1")]
    public class ArchDoc
    {
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature;
    }

    [XmlRoot(ElementName = "DocBaseInfo", Namespace = "urn:customs.ru:Information:EArchDocuments:ArchAddDocRequest:5.13.1")]
    public class DocBaseInfo
    {
        [XmlElement(ElementName = "PrDocumentName", Namespace = "urn:customs.ru:Information:EArchDocuments:EADCommonTypes:5.13.1")]
        public object PrDocumentName;

        [XmlElement(ElementName = "PrDocumentNumber", Namespace = "urn:customs.ru:Information:EArchDocuments:EADCommonTypes:5.13.1")]
        public object PrDocumentNumber;

        [XmlElement(ElementName = "PrDocumentDate", Namespace = "urn:customs.ru:Information:EArchDocuments:EADCommonTypes:5.13.1")]
        public object PrDocumentDate;
    }

    [XmlRoot(ElementName = "ArchAddDocRequest", Namespace = "urn:customs.ru:Information:EArchDocuments:ArchAddDocRequest:5.13.1")]
    public class ArchAddDocRequest
    {
        [XmlElement(ElementName = "DocumentID", Namespace = "urn:customs.ru:Information:EArchDocuments:EADCommonTypes:5.13.1")]
        public string DocumentID;

        [XmlElement(ElementName = "ArchDeclID", Namespace = "urn:customs.ru:Information:EArchDocuments:EADCommonTypes:5.13.1")]
        public object ArchDeclID;

        [XmlElement(ElementName = "ArchID", Namespace = "urn:customs.ru:Information:EArchDocuments:EADCommonTypes:5.13.1")]
        public object ArchID;

        [XmlElement(ElementName = "DocCode", Namespace = "urn:customs.ru:Information:EArchDocuments:ArchAddDocRequest:5.13.1")]
        public object DocCode;

        [XmlElement(ElementName = "ArchDoc", Namespace = "urn:customs.ru:Information:EArchDocuments:ArchAddDocRequest:5.13.1")]
        public ArchDoc ArchDoc;

        [XmlElement(ElementName = "DocBaseInfo", Namespace = "urn:customs.ru:Information:EArchDocuments:ArchAddDocRequest:5.13.1")]
        public DocBaseInfo DocBaseInfo;

        [XmlAttribute(AttributeName = "xmlns", Namespace = "")]
        public string Xmlns;

        [XmlAttribute(AttributeName = "ct", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ct;

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi;

        [XmlAttribute(AttributeName = "DocumentModeID", Namespace = "")]
        public string DocumentModeID;

        [XmlText]
        public string Text;
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://www.w3.org/2001/06/soap-envelope")]
    public class Body
    {
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature;
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://www.w3.org/2001/06/soap-envelope")]
    public class Envelope
    {
        [XmlElement(ElementName = "Header", Namespace = "http://www.w3.org/2001/06/soap-envelope")]
        public Header Header;

        [XmlElement(ElementName = "Body", Namespace = "http://www.w3.org/2001/06/soap-envelope")]
        public Body Body;

        [XmlAttribute(AttributeName = "xmlns", Namespace = "")]
        public string Xmlns;

        [XmlAttribute(AttributeName = "app", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string App;

        [XmlAttribute(AttributeName = "att", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Att;

        [XmlAttribute(AttributeName = "roi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Roi;

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi;

        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation;

        [XmlText]
        public string Text;
    }
}