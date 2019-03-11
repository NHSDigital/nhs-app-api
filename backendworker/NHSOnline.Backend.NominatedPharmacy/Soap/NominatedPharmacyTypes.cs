using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.NominatedPharmacy.Soap
{
    public class NominatedPharmacyTypes
    {
        public class From
        {
            [XmlElement(ElementName = "Address", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
            public string Address { get; set; }
        }

        public class Id
        {
            [XmlAttribute(AttributeName = "root")]
            public string Root { get; set; }
            [XmlAttribute(AttributeName = "extension")]
            public string Extension { get; set; }
        }

        public class Device
        {
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "determinerCode")]
            public string DeterminerCode { get; set; }
        }

        public class CommunicationFunctionRcv
        {
            [XmlElement(ElementName = "device", Namespace = "urn:hl7-org:v3")]
            public Device Device { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class CommunicationFunctionSnd
        {
            [XmlElement(ElementName = "device", Namespace = "urn:hl7-org:v3")]
            public Device Device { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class Header
        {
            [XmlElement(ElementName = "MessageID", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
            public string MessageID { get; set; }
            [XmlElement(ElementName = "Action", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
            public string Action { get; set; }
            [XmlElement(ElementName = "To", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
            public string To { get; set; }
            [XmlElement(ElementName = "From", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
            public From From { get; set; }
            [XmlElement(ElementName = "communicationFunctionRcv", Namespace = "urn:hl7-org:v3")]
            public CommunicationFunctionRcv CommunicationFunctionRcv { get; set; }
            [XmlElement(ElementName = "communicationFunctionSnd", Namespace = "urn:hl7-org:v3")]
            public CommunicationFunctionSnd CommunicationFunctionSnd { get; set; }
            [XmlElement(ElementName = "RelatesTo", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
            public string RelatesTo { get; set; }
        }

        public class CreationTime
        {
            [XmlAttribute(AttributeName = "value")]
            public string Value { get; set; }
        }

        public class VersionCode
        {
            [XmlAttribute(AttributeName = "code")]
            public string Code { get; set; }
        }

        public class InteractionId
        {
            [XmlAttribute(AttributeName = "root")]
            public string Root { get; set; }
            [XmlAttribute(AttributeName = "extension")]
            public string Extension { get; set; }
        }

        public class ProcessingCode
        {
            [XmlAttribute(AttributeName = "code")]
            public string Code { get; set; }
        }

        public class ProcessingModeCode
        {
            [XmlAttribute(AttributeName = "code")]
            public string Code { get; set; }
        }

        public class AcceptAckCode
        {
            [XmlAttribute(AttributeName = "code")]
            public string Code { get; set; }
        }

        public class MessageRef
        {
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
        }

        public class Acknowledgement
        {
            [XmlElement(ElementName = "messageRef", Namespace = "urn:hl7-org:v3")]
            public MessageRef MessageRef { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class AgentSystemSDSInner
        {
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "determinerCode")]
            public string DeterminerCode { get; set; }
        }

        public class AgentSystemSDS
        {
            [XmlElement(ElementName = "agentSystemSDS", Namespace = "urn:hl7-org:v3")]
            public AgentSystemSDSInner AgentSystemSDSInner { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
        }

        public class Author1
        {
            [XmlElement(ElementName = "AgentSystemSDS", Namespace = "urn:hl7-org:v3")]
            public AgentSystemSDS AgentSystemSDS { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class Code
        {
            [XmlAttribute(AttributeName = "code")]
            public string _code { get; set; }
            [XmlAttribute(AttributeName = "codeSystem")]
            public string CodeSystem { get; set; }
        }

        public class Value
        {
            [XmlAttribute(AttributeName = "value")]
            public string _value { get; set; }
            [XmlAttribute(AttributeName = "codeSystem")]
            public string CodeSystem { get; set; }
            [XmlAttribute(AttributeName = "code")]
            public string Code { get; set; }

            [XmlAttribute(AttributeName = "root")]
            public string Root { get; set; }
            [XmlAttribute(AttributeName = "extension")]
            public string Extension { get; set; }
        }

        public class PertinentSerialChangeNumber
        {
            [XmlElement(ElementName = "code", Namespace = "urn:hl7-org:v3")]
            public Code Code { get; set; }
            [XmlElement(ElementName = "value", Namespace = "urn:hl7-org:v3")]
            public Value Value { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "moodCode")]
            public string MoodCode { get; set; }
        }

        public class PertinentInformation
        {
            [XmlElement(ElementName = "pertinentSerialChangeNumber", Namespace = "urn:hl7-org:v3")]
            public PertinentSerialChangeNumber PertinentSerialChangeNumber { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class AdministrativeGenderCode
        {
            [XmlAttribute(AttributeName = "code")]
            public string Code { get; set; }
        }

        public class BirthTime
        {
            [XmlAttribute(AttributeName = "value")]
            public string Value { get; set; }
        }

        public class Low
        {
            [XmlAttribute(AttributeName = "value")]
            public string Value { get; set; }
        }

        public class EffectiveTime
        {
            [XmlElement(ElementName = "low", Namespace = "urn:hl7-org:v3")]
            public Low Low { get; set; }
        }

        public class AssignedEntity
        {
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
        }

        public class Performer
        {
            [XmlElement(ElementName = "assignedEntity", Namespace = "urn:hl7-org:v3")]
            public AssignedEntity AssignedEntity { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class PatientCareProvisionEvent
        {
            [XmlElement(ElementName = "code", Namespace = "urn:hl7-org:v3")]
            public Code Code { get; set; }
            [XmlElement(ElementName = "effectiveTime", Namespace = "urn:hl7-org:v3")]
            public EffectiveTime EffectiveTime { get; set; }
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlElement(ElementName = "performer", Namespace = "urn:hl7-org:v3")]
            public Performer Performer { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "moodCode")]
            public string MoodCode { get; set; }
        }

        public class SubjectOf
        {
            [XmlElement(ElementName = "patientCareProvisionEvent", Namespace = "urn:hl7-org:v3")]
            public PatientCareProvisionEvent PatientCareProvisionEvent { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class PlayedOtherProviderPatient
        {
            [XmlElement(ElementName = "subjectOf", Namespace = "urn:hl7-org:v3")]
            public SubjectOf SubjectOf { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
        }

        public class UseablePeriod
        {
            [XmlElement(ElementName = "low", Namespace = "urn:hl7-org:v3")]
            public Low Low { get; set; }
        }

        public class Addr
        {
            [XmlElement(ElementName = "streetAddressLine", Namespace = "urn:hl7-org:v3")]
            public List<string> StreetAddressLine { get; set; }
            [XmlElement(ElementName = "postalCode", Namespace = "urn:hl7-org:v3")]
            public string PostalCode { get; set; }
            [XmlElement(ElementName = "addressKey", Namespace = "urn:hl7-org:v3")]
            public string AddressKey { get; set; }
            [XmlElement(ElementName = "useablePeriod", Namespace = "urn:hl7-org:v3")]
            public UseablePeriod UseablePeriod { get; set; }
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlAttribute(AttributeName = "use")]
            public string Use { get; set; }
        }

        public class COCT_MT000201UK02PartOfWhole
        {
            [XmlElement(ElementName = "addr", Namespace = "urn:hl7-org:v3")]
            public Addr Addr { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
        }

        public class ValidTime
        {
            [XmlElement(ElementName = "low", Namespace = "urn:hl7-org:v3")]
            public Low Low { get; set; }
        }

        public class Name
        {
            [XmlElement(ElementName = "prefix", Namespace = "urn:hl7-org:v3")]
            public string Prefix { get; set; }
            [XmlElement(ElementName = "given", Namespace = "urn:hl7-org:v3")]
            public List<string> Given { get; set; }
            [XmlElement(ElementName = "family", Namespace = "urn:hl7-org:v3")]
            public string Family { get; set; }
            [XmlElement(ElementName = "validTime", Namespace = "urn:hl7-org:v3")]
            public ValidTime ValidTime { get; set; }
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlAttribute(AttributeName = "use")]
            public string Use { get; set; }
        }

        public class PartPerson
        {
            [XmlElement(ElementName = "name", Namespace = "urn:hl7-org:v3")]
            public Name Name { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "determinerCode")]
            public string DeterminerCode { get; set; }
        }

        [XmlRoot(ElementName = "COCT_MT000203UK02.PartOfWhole", Namespace = "urn:hl7-org:v3")]
        public class COCT_MT000203UK02PartOfWhole
        {
            [XmlElement(ElementName = "partPerson", Namespace = "urn:hl7-org:v3")]
            public PartPerson PartPerson { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
        }

        public class PatientPerson
        {
            [XmlElement(ElementName = "administrativeGenderCode", Namespace = "urn:hl7-org:v3")]
            public AdministrativeGenderCode AdministrativeGenderCode { get; set; }
            [XmlElement(ElementName = "birthTime", Namespace = "urn:hl7-org:v3")]
            public BirthTime BirthTime { get; set; }
            [XmlElement(ElementName = "playedOtherProviderPatient", Namespace = "urn:hl7-org:v3")]
            public PlayedOtherProviderPatient PlayedOtherProviderPatient { get; set; }
            [XmlElement(ElementName = "COCT_MT000201UK02.PartOfWhole", Namespace = "urn:hl7-org:v3")]
            public COCT_MT000201UK02PartOfWhole COCT_MT000201UK02PartOfWhole { get; set; }
            [XmlElement(ElementName = "COCT_MT000203UK02.PartOfWhole", Namespace = "urn:hl7-org:v3")]
            public COCT_MT000203UK02PartOfWhole COCT_MT000203UK02PartOfWhole { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "determinerCode")]
            public string DeterminerCode { get; set; }
        }

        public class PreviousNhsContact
        {
            [XmlElement(ElementName = "code", Namespace = "urn:hl7-org:v3")]
            public Code Code { get; set; }
            [XmlElement(ElementName = "value", Namespace = "urn:hl7-org:v3")]
            public Value Value { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "moodCode")]
            public string MoodCode { get; set; }
        }

        public class SubjectOf8
        {
            [XmlElement(ElementName = "previousNhsContact", Namespace = "urn:hl7-org:v3")]
            public PreviousNhsContact PreviousNhsContact { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class PatientRole
        {
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlElement(ElementName = "patientPerson", Namespace = "urn:hl7-org:v3")]
            public PatientPerson PatientPerson { get; set; }
            [XmlElement(ElementName = "subjectOf8", Namespace = "urn:hl7-org:v3")]
            public SubjectOf8 SubjectOf8 { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
        }

        public class Subject
        {
            [XmlElement(ElementName = "PDSResponse", Namespace = "urn:hl7-org:v3")]
            public PDSResponse PDSResponse { get; set; }
            [XmlElement(ElementName = "patientRole", Namespace = "urn:hl7-org:v3")]
            public PatientRole PatientRole { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class PDSResponse
        {
            [XmlElement(ElementName = "pertinentInformation", Namespace = "urn:hl7-org:v3")]
            public PertinentInformation PertinentInformation { get; set; }
            [XmlElement(ElementName = "subject", Namespace = "urn:hl7-org:v3")]
            public Subject Subject { get; set; }
            [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsi { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "moodCode")]
            public string MoodCode { get; set; }
        }

        public class QueryResponseCode
        {
            [XmlAttribute(AttributeName = "code")]
            public string Code { get; set; }
        }

        public class QueryAck
        {
            [XmlElement(ElementName = "queryResponseCode", Namespace = "urn:hl7-org:v3")]
            public QueryResponseCode QueryResponseCode { get; set; }
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }
        }

        public class ControlActEvent
        {
            [XmlElement(ElementName = "author", Namespace = "urn:hl7-org:v3")]
            public Author Author { get; set; }
            [XmlElement(ElementName = "author1", Namespace = "urn:hl7-org:v3")]
            public Author1 Author1 { get; set; }
            [XmlElement(ElementName = "subject", Namespace = "urn:hl7-org:v3")]
            public Subject Subject { get; set; }
            [XmlElement(ElementName = "queryAck", Namespace = "urn:hl7-org:v3")]
            public QueryAck QueryAck { get; set; }
            [XmlElement(ElementName = "query", Namespace = "urn:hl7-org:v3")]
            public Query Query { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "moodCode")]
            public string MoodCode { get; set; }

        }

        public class QUPA_IN000009UK03_Response
        {
            [XmlElement(ElementName = "QUPA_IN000009UK03", Namespace = "urn:hl7-org:v3")]
            public QUPA_IN000009UK03 QUPA_IN000009UK03 { get; set; }
        }

        public class QUPA_IN000009UK03
        {
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlElement(ElementName = "creationTime", Namespace = "urn:hl7-org:v3")]
            public CreationTime CreationTime { get; set; }
            [XmlElement(ElementName = "versionCode", Namespace = "urn:hl7-org:v3")]
            public VersionCode VersionCode { get; set; }
            [XmlElement(ElementName = "interactionId", Namespace = "urn:hl7-org:v3")]
            public InteractionId InteractionId { get; set; }
            [XmlElement(ElementName = "processingCode", Namespace = "urn:hl7-org:v3")]
            public ProcessingCode ProcessingCode { get; set; }
            [XmlElement(ElementName = "processingModeCode", Namespace = "urn:hl7-org:v3")]
            public ProcessingModeCode ProcessingModeCode { get; set; }
            [XmlElement(ElementName = "acceptAckCode", Namespace = "urn:hl7-org:v3")]
            public AcceptAckCode AcceptAckCode { get; set; }
            [XmlElement(ElementName = "acknowledgement", Namespace = "urn:hl7-org:v3")]
            public Acknowledgement Acknowledgement { get; set; }
            [XmlElement(ElementName = "communicationFunctionRcv", Namespace = "urn:hl7-org:v3")]
            public CommunicationFunctionRcv CommunicationFunctionRcv { get; set; }
            [XmlElement(ElementName = "communicationFunctionSnd", Namespace = "urn:hl7-org:v3")]
            public CommunicationFunctionSnd CommunicationFunctionSnd { get; set; }
            [XmlElement(ElementName = "ControlActEvent", Namespace = "urn:hl7-org:v3")]
            public ControlActEvent ControlActEvent { get; set; }
        }
        
        [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public class Body<T>
        {
            [XmlElement(ElementName = "retrievalQueryResponse", Namespace = "urn:hl7-org:v3")]
            public T RetrievalQueryResponse { get; set; }
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

        public class AgentPersonSDS
        {
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlElement(ElementName = "agentPersonSDS", Namespace = "urn:hl7-org:v3")]
            public AgentPersonSDSInner AgentPersonSDSInner { get; set; }
            [XmlElement(ElementName = "part", Namespace = "urn:hl7-org:v3")]
            public Part Part { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
        }

        public class Author
        {
            [XmlElement(ElementName = "AgentPersonSDS", Namespace = "urn:hl7-org:v3")]
            public AgentPersonSDS AgentPersonSDS { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class HistoricDataIndicator
        {
            [XmlElement(ElementName = "semanticsText", Namespace = "urn:hl7-org:v3")]
            public string SemanticsText { get; set; }
            [XmlElement(ElementName = "value", Namespace = "urn:hl7-org:v3")]
            public Value Value { get; set; }
        }

        public class PersonId
        {
            [XmlElement(ElementName = "semanticsText", Namespace = "urn:hl7-org:v3")]
            public string SemanticsText { get; set; }
            [XmlElement(ElementName = "value", Namespace = "urn:hl7-org:v3")]
            public Value Value { get; set; }
        }

        public class RetrievalItem
        {
            [XmlElement(ElementName = "semanticsText", Namespace = "urn:hl7-org:v3")]
            public string SemanticsText { get; set; }
        }

        public class Query
        {
            [XmlElement(ElementName = "historicDataIndicator", Namespace = "urn:hl7-org:v3")]
            public HistoricDataIndicator HistoricDataIndicator { get; set; }
            [XmlElement(ElementName = "person.id", Namespace = "urn:hl7-org:v3")]
            public PersonId PersonId { get; set; }
            [XmlElement(ElementName = "retrievalItem", Namespace = "urn:hl7-org:v3")]
            public RetrievalItem RetrievalItem { get; set; }
        }

        [XmlRoot(ElementName = "QUPA_IN000008UK02", Namespace = "urn:hl7-org:v3")]
        public class QUPA_IN000008UK02
        {
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public Id Id { get; set; }
            [XmlElement(ElementName = "creationTime", Namespace = "urn:hl7-org:v3")]
            public CreationTime CreationTime { get; set; }
            [XmlElement(ElementName = "versionCode", Namespace = "urn:hl7-org:v3")]
            public VersionCode VersionCode { get; set; }
            [XmlElement(ElementName = "interactionId", Namespace = "urn:hl7-org:v3")]
            public InteractionId InteractionId { get; set; }
            [XmlElement(ElementName = "processingCode", Namespace = "urn:hl7-org:v3")]
            public ProcessingCode ProcessingCode { get; set; }
            [XmlElement(ElementName = "processingModeCode", Namespace = "urn:hl7-org:v3")]
            public ProcessingModeCode ProcessingModeCode { get; set; }
            [XmlElement(ElementName = "acceptAckCode", Namespace = "urn:hl7-org:v3")]
            public AcceptAckCode AcceptAckCode { get; set; }
            [XmlElement(ElementName = "communicationFunctionRcv", Namespace = "urn:hl7-org:v3")]
            public CommunicationFunctionRcv CommunicationFunctionRcv { get; set; }
            [XmlElement(ElementName = "communicationFunctionSnd", Namespace = "urn:hl7-org:v3")]
            public CommunicationFunctionSnd CommunicationFunctionSnd { get; set; }
            [XmlElement(ElementName = "ControlActEvent", Namespace = "urn:hl7-org:v3")]
            public ControlActEvent ControlActEvent { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }
    }
}
