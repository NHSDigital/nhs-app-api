using System.Xml.Serialization;

namespace NHSOnline.Backend.NominatedPharmacy.Soap
{
    public static class NominatedPharmacyTypes
    {
        public class Id
        {
            [XmlAttribute(AttributeName = "root")]
            public string Root { get; set; }
            [XmlAttribute(AttributeName = "extension")]
            public string Extension { get; set; }
        }
        
        public class CreationTime
        {
            [XmlAttribute(AttributeName = "value")]
            public string Value { get; set; }
        }
        
        public class VersionCode {
            [XmlAttribute(AttributeName="code")]
            public string Code { get; set; }
        }
        
        public class InteractionId
        {
            [XmlAttribute(AttributeName = "root")]
            public string Root { get; set; }
            [XmlAttribute(AttributeName = "extension")]
            public string Extension { get; set; }
        }
        
        public class ProcessingCode {
            [XmlAttribute(AttributeName="code")]
            public string Code { get; set; }
        }
        
        public class ProcessingModeCode
        {
            [XmlAttribute(AttributeName = "code")]
            public string Code { get; set; }
        }
        
        [XmlRoot(ElementName="acceptAckCode", Namespace="urn:hl7-org:v3")]
        public class AcceptAckCode {
            [XmlAttribute(AttributeName="code")]
            public string Code { get; set; }
        }
        
        [XmlRoot(ElementName="device", Namespace="urn:hl7-org:v3")]
        public class Device {
            [XmlElement(ElementName="id", Namespace="urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlAttribute(AttributeName="classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName="determinerCode")]
            public string DeterminerCode { get; set; }
        }
        
        public class CodeElement
        {
            [XmlAttribute(AttributeName = "code")]
            public string Code { get; set; }
            [XmlAttribute(AttributeName = "codeSystem")]
            public string CodeSystem { get; set; }
        }
        
        public class AgentPersonSDSInner
        {
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "determinerCode")]
            public string DeterminerCode { get; set; }
        }
        
        public class PartSDSRole
        {
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
        }
        
        public class Part
        {
            [XmlElement(ElementName = "partSDSRole", Namespace = "urn:hl7-org:v3")]
            public PartSDSRole PartSDSRole { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }
        
        public class AgentPersonSDS {
            [XmlElement(ElementName="id", Namespace="urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlElement(ElementName="agentPersonSDS", Namespace="urn:hl7-org:v3")]
            public AgentPersonSDSInner AgentPersonSDSInner { get; set; }
            [XmlElement(ElementName="part", Namespace="urn:hl7-org:v3")]
            public Part Part { get; set; }
            [XmlAttribute(AttributeName="classCode")]
            public string ClassCode { get; set; }
        }
        
        public class AgentSystemSDSInner {
            [XmlElement(ElementName="id", Namespace="urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlAttribute(AttributeName="classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName="determinerCode")]
            public string DeterminerCode { get; set; }
        }

        public class AgentSystemSDS {
            [XmlElement(ElementName="agentSystemSDS", Namespace="urn:hl7-org:v3")]
            public AgentSystemSDSInner AgentSystemSDSInner { get; set; }
            [XmlAttribute(AttributeName="classCode")]
            public string ClassCode { get; set; }
        }

        public class Author1
        {
            [XmlElement(ElementName = "AgentSystemSDS", Namespace = "urn:hl7-org:v3")]
            public AgentSystemSDS AgentSystemSDS { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }
        
        public class CommunicationFunctionRcv
        {
            [XmlElement(ElementName = "device", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.Device Device { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }
        
        public class CommunicationFunctionSnd
        {
            [XmlElement(ElementName = "device", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.Device Device { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }


    }
}
