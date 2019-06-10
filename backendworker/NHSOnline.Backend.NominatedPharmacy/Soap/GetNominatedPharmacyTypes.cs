using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.NominatedPharmacy.Soap
{
    public static class GetNominatedPharmacyTypes
    {
        public class From
        {
            [XmlElement(ElementName = "Address")]
            public string Address { get; set; }
        }

        public class Header
        {
            [XmlElement(ElementName = "MessageID")]
            public string MessageID { get; set; }
            [XmlElement(ElementName = "Action")]
            public string Action { get; set; }
            [XmlElement(ElementName = "To")]
            public string To { get; set; }
            [XmlElement(ElementName = "From")]
            public From From { get; set; }
            [XmlElement(ElementName = "communicationFunctionRcv")]
            public NominatedPharmacyTypes.CommunicationFunctionRcv CommunicationFunctionRcv { get; set; }
            [XmlElement(ElementName = "communicationFunctionSnd")]
            public NominatedPharmacyTypes.CommunicationFunctionSnd CommunicationFunctionSnd { get; set; }
            [XmlElement(ElementName = "RelatesTo")]
            public string RelatesTo { get; set; }
        }

        public class MessageRef
        {
            [XmlElement(ElementName = "id")]
            public NominatedPharmacyTypes.Id Id { get; set; }
        }

        public class Acknowledgement
        {
            [XmlElement(ElementName = "messageRef")]
            public MessageRef MessageRef { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class ValueElement
        {
            [XmlAttribute(AttributeName = "value")]
            public string Value { get; set; }
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
            [XmlElement(ElementName = "code")]
            public NominatedPharmacyTypes.CodeElement Code { get; set; }
            [XmlElement(ElementName = "value")]
            public ValueElement Value { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "moodCode")]
            public string MoodCode { get; set; }
        }

        public class PertinentInformation
        {
            [XmlElement(ElementName = "pertinentSerialChangeNumber")]
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
            [XmlElement(ElementName = "low")]
            public Low Low { get; set; }
        }

        public class AssignedEntity
        {
            [XmlElement(ElementName = "id")]
            public NominatedPharmacyTypes.Id Id { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
        }

        public class Performer
        {
            [XmlElement(ElementName = "assignedEntity")]
            public AssignedEntity AssignedEntity { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class PatientCareProvisionEvent
        {
            [XmlElement(ElementName = "code")]
            public NominatedPharmacyTypes.CodeElement Code { get; set; }
            [XmlElement(ElementName = "effectiveTime")]
            public EffectiveTime EffectiveTime { get; set; }
            [XmlElement(ElementName = "id")]
            public NominatedPharmacyTypes.Id Id { get; set; }
            [XmlElement(ElementName = "performer")]
            public Performer Performer { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "moodCode")]
            public string MoodCode { get; set; }
        }

        public class SubjectOf
        {
            [XmlElement(ElementName = "patientCareProvisionEvent")]
            public PatientCareProvisionEvent PatientCareProvisionEvent { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class PlayedOtherProviderPatient
        {
            [XmlElement(ElementName = "subjectOf")]
            public SubjectOf SubjectOf { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
        }

        public class UseablePeriod
        {
            [XmlElement(ElementName = "low")]
            public Low Low { get; set; }
        }

        public class Addr
        {
            [XmlElement(ElementName = "streetAddressLine")]
            public List<string> StreetAddressLine { get; set; }
            [XmlElement(ElementName = "postalCode")]
            public string PostalCode { get; set; }
            [XmlElement(ElementName = "addressKey")]
            public string AddressKey { get; set; }
            [XmlElement(ElementName = "useablePeriod")]
            public UseablePeriod UseablePeriod { get; set; }
            [XmlElement(ElementName = "id")]
            public NominatedPharmacyTypes.Id Id { get; set; }
            [XmlAttribute(AttributeName = "use")]
            public string Use { get; set; }
        }

        public class COCTMT000201UK02PartOfWhole
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
            [XmlElement(ElementName = "prefix")]
            public string Prefix { get; set; }
            [XmlElement(ElementName = "given")]
            public List<string> Given { get; set; }
            [XmlElement(ElementName = "family")]
            public string Family { get; set; }
            [XmlElement(ElementName = "validTime")]
            public ValidTime ValidTime { get; set; }
            [XmlElement(ElementName = "id")]
            public NominatedPharmacyTypes.Id Id { get; set; }
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
        public class COCTMT000203UK02PartOfWhole
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
            public List<PlayedOtherProviderPatient> PlayedOtherProviderPatients { get; set; }

            [XmlElement(ElementName = "COCT_MT000201UK02.PartOfWhole", Namespace = "urn:hl7-org:v3")]
            public COCTMT000201UK02PartOfWhole COCTMT000201UK02PartOfWhole { get; set; }

            [XmlElement(ElementName = "COCT_MT000203UK02.PartOfWhole", Namespace = "urn:hl7-org:v3")]
            public COCTMT000203UK02PartOfWhole COCTMT000203UK02PartOfWhole { get; set; }

            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }

            [XmlAttribute(AttributeName = "determinerCode")]
            public string DeterminerCode { get; set; }
        }

        public class PreviousNhsContact
        {
            [XmlElement(ElementName = "code", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.CodeElement Code { get; set; }
            [XmlElement(ElementName = "value", Namespace = "urn:hl7-org:v3")]
            public ValueElement Value { get; set; }
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
            [XmlElement(ElementName = "id")]
            public NominatedPharmacyTypes.Id Id { get; set; }
            [XmlElement(ElementName = "patientPerson")]
            public PatientPerson PatientPerson { get; set; }
            [XmlElement(ElementName = "subjectOf8")]
            public SubjectOf8 SubjectOf8 { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
        }

        public class Subject
        {
            [XmlElement(ElementName = "PDSResponse")]
            public PDSResponse PDSResponse { get; set; }
            [XmlElement(ElementName = "patientRole")]
            public PatientRole PatientRole { get; set; }
            [XmlAttribute(AttributeName = "typeCode")]
            public string TypeCode { get; set; }
        }

        public class PDSResponse
        {
            [XmlElement(ElementName = "pertinentInformation")]
            public PertinentInformation PertinentInformation { get; set; }
            [XmlElement(ElementName = "subject")]
            public Subject Subject { get; set; }
            [XmlAttribute(AttributeName = "xsi")]
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
            [XmlElement(ElementName = "queryResponseCode")]
            public QueryResponseCode QueryResponseCode { get; set; }
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }
        }

        public class ControlActEvent
        {
            [XmlElement(ElementName = "author1", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.Author1 Author1 { get; set; }
            [XmlElement(ElementName = "subject", Namespace = "urn:hl7-org:v3")]
            public Subject Subject { get; set; }
            [XmlElement(ElementName = "queryAck", Namespace = "urn:hl7-org:v3")]
            public QueryAck QueryAck { get; set; }
            [XmlElement(ElementName = "query", Namespace = "urn:hl7-org:v3")]
            public QueryElement Query { get; set; }
            [XmlAttribute(AttributeName = "classCode")]
            public string ClassCode { get; set; }
            [XmlAttribute(AttributeName = "moodCode")]
            public string MoodCode { get; set; }

        }

        public class QUPAIN000009UK03Response
        {
            [XmlElement(ElementName = "QUPA_IN000009UK03", Namespace = "urn:hl7-org:v3")]
            public QUPAIN000009UK03 QUPAIN000009UK03 { get; set; }
        }

        public class QUPAIN000009UK03
        {
            [XmlElement(ElementName = "id", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.Id Id { get; set; }
            [XmlElement(ElementName = "creationTime", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.CreationTime CreationTime { get; set; }
            [XmlElement(ElementName = "versionCode", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.VersionCode VersionCode { get; set; }
            [XmlElement(ElementName = "interactionId", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.InteractionId InteractionId { get; set; }
            [XmlElement(ElementName = "processingCode", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.ProcessingCode ProcessingCode { get; set; }
            [XmlElement(ElementName = "processingModeCode", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.ProcessingModeCode ProcessingModeCode { get; set; }
            [XmlElement(ElementName = "acceptAckCode", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.AcceptAckCode AcceptAckCode { get; set; }
            [XmlElement(ElementName = "acknowledgement", Namespace = "urn:hl7-org:v3")]
            public Acknowledgement Acknowledgement { get; set; }
            [XmlElement(ElementName = "communicationFunctionRcv", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.CommunicationFunctionRcv CommunicationFunctionRcv { get; set; }
            [XmlElement(ElementName = "communicationFunctionSnd", Namespace = "urn:hl7-org:v3")]
            public NominatedPharmacyTypes.CommunicationFunctionSnd CommunicationFunctionSnd { get; set; }
            [XmlElement(ElementName = "ControlActEvent", Namespace = "urn:hl7-org:v3")]
            public ControlActEvent ControlActEvent { get; set; }
        }
        
        [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public class Body<T>
        {
            [XmlElement(ElementName = "retrievalQueryResponse", Namespace = "urn:hl7-org:v3")]
            public T RetrievalQueryResponse { get; set; }
        }
        

        public class HistoricDataIndicator
        {
            [XmlElement(ElementName = "semanticsText", Namespace = "urn:hl7-org:v3")]
            public string SemanticsText { get; set; }
            [XmlElement(ElementName = "value", Namespace = "urn:hl7-org:v3")]
            public ValueElement Value { get; set; }
        }

        public class PersonId
        {
            [XmlElement(ElementName = "semanticsText", Namespace = "urn:hl7-org:v3")]
            public string SemanticsText { get; set; }
            [XmlElement(ElementName = "value", Namespace = "urn:hl7-org:v3")]
            public ValueElement Value { get; set; }
        }

        public class RetrievalItem
        {
            [XmlElement(ElementName = "semanticsText", Namespace = "urn:hl7-org:v3")]
            public string SemanticsText { get; set; }
        }

        public class QueryElement
        {
            [XmlElement(ElementName = "historicDataIndicator", Namespace = "urn:hl7-org:v3")]
            public HistoricDataIndicator HistoricDataIndicator { get; set; }
            [XmlElement(ElementName = "person.id", Namespace = "urn:hl7-org:v3")]
            public PersonId PersonId { get; set; }
            [XmlElement(ElementName = "retrievalItem", Namespace = "urn:hl7-org:v3")]
            public RetrievalItem RetrievalItem { get; set; }
        }

        [XmlRoot(ElementName = "QUPA_IN000008UK02", Namespace = "urn:hl7-org:v3")]
        public class QUPAIN000008UK02
        {
            [XmlElement(ElementName = "id")]
            public NominatedPharmacyTypes.Id Id { get; set; }
            [XmlElement(ElementName = "creationTime")]
            public NominatedPharmacyTypes.CreationTime CreationTime { get; set; }
            [XmlElement(ElementName = "versionCode")]
            public NominatedPharmacyTypes.VersionCode VersionCode { get; set; }
            [XmlElement(ElementName = "interactionId")]
            public NominatedPharmacyTypes.InteractionId InteractionId { get; set; }
            [XmlElement(ElementName = "processingCode")]
            public NominatedPharmacyTypes.ProcessingCode ProcessingCode { get; set; }
            [XmlElement(ElementName = "processingModeCode")]
            public NominatedPharmacyTypes.ProcessingModeCode ProcessingModeCode { get; set; }
            [XmlElement(ElementName = "acceptAckCode")]
            public NominatedPharmacyTypes.AcceptAckCode AcceptAckCode { get; set; }
            [XmlElement(ElementName = "communicationFunctionRcv")]
            public NominatedPharmacyTypes.CommunicationFunctionRcv CommunicationFunctionRcv { get; set; }
            [XmlElement(ElementName = "communicationFunctionSnd")]
            public NominatedPharmacyTypes.CommunicationFunctionSnd CommunicationFunctionSnd { get; set; }
            [XmlElement(ElementName = "ControlActEvent")]
            public ControlActEvent ControlActEvent { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }
    }
}


