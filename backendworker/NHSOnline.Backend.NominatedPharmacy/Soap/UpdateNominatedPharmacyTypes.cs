using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace NHSOnline.Backend.NominatedPharmacy.Soap
{
    public static class UpdateNominatedPharmacyTypes
    {
		public class ValueElement {
			[XmlAttribute(AttributeName="value")]
			public string value { get; set; }
		}

		public class PertinentSerialChangeNumber {
			[XmlElement(ElementName="code", Namespace="urn:hl7-org:v3")]
			public NominatedPharmacyTypes.CodeElement Code { get; set; }
			[XmlElement(ElementName="value", Namespace="urn:hl7-org:v3")]
			public ValueElement Value { get; set; }
			[XmlAttribute(AttributeName="classCode")]
			public string ClassCode { get; set; }
			[XmlAttribute(AttributeName="moodCode")]
			public string MoodCode { get; set; }
		}
	
		public class PertinentInformation {
			[XmlElement(ElementName="pertinentSerialChangeNumber", Namespace="urn:hl7-org:v3")]
			public PertinentSerialChangeNumber PertinentSerialChangeNumber { get; set; }
			[XmlAttribute(AttributeName="typeCode")]
			public string TypeCode { get; set; }
		}
		
		public class AssignedOrganization {
			[XmlElement(ElementName="id", Namespace="urn:hl7-org:v3")]
			public NominatedPharmacyTypes.Id Id { get; set; }
			[XmlAttribute(AttributeName="classCode")]
			public string ClassCode { get; set; }
		}

		public class Performer {
			[XmlElement(ElementName="assignedOrganization", Namespace="urn:hl7-org:v3")]
			public AssignedOrganization AssignedOrganization { get; set; }
			[XmlAttribute(AttributeName="typeCode")]
			public string TypeCode { get; set; }
		}

		public class PatientCareProvision {
			[XmlElement(ElementName="code", Namespace="urn:hl7-org:v3")]
			public NominatedPharmacyTypes.CodeElement Code { get; set; }
			[XmlElement(ElementName="id", Namespace="urn:hl7-org:v3")]
			public NominatedPharmacyTypes.Id Id { get; set; }
			[XmlElement(ElementName="performer", Namespace="urn:hl7-org:v3")]
			public Performer Performer { get; set; }
			[XmlAttribute(AttributeName="classCode")]
			public string ClassCode { get; set; }
			[XmlAttribute(AttributeName="moodCode")]
			public string MoodCode { get; set; }
			[XmlAttribute(AttributeName="updateMode")]
			public string UpdateMode { get; set; }
		}
	
		public class SubjectOf {
			[XmlElement(ElementName="patientCareProvision", Namespace="urn:hl7-org:v3")]
			public PatientCareProvision PatientCareProvision { get; set; }
			[XmlAttribute(AttributeName="typeCode")]
			public string TypeCode { get; set; }
		}

		public class PlayedOtherProviderPatient {
			[XmlElement(ElementName="subjectOf", Namespace="urn:hl7-org:v3")]
			public SubjectOf SubjectOf { get; set; }
			[XmlAttribute(AttributeName="classCode")]
			public string ClassCode { get; set; }
		}

		public class PatientPerson {
			[XmlElement(ElementName="playedOtherProviderPatient", Namespace="urn:hl7-org:v3")]
			public PlayedOtherProviderPatient PlayedOtherProviderPatient { get; set; }
			[XmlAttribute(AttributeName="classCode")]
			public string ClassCode { get; set; }
			[XmlAttribute(AttributeName="determinerCode")]
			public string DeterminerCode { get; set; }
		}
	
		public class PatientRole {
			[XmlElement(ElementName="id", Namespace="urn:hl7-org:v3")]
			public NominatedPharmacyTypes.Id Id { get; set; }
			[XmlElement(ElementName="patientPerson", Namespace="urn:hl7-org:v3")]
			public PatientPerson PatientPerson { get; set; }
			[XmlAttribute(AttributeName="classCode")]
			public string ClassCode { get; set; }
		}

		public class Subject {
			[XmlElement(ElementName="patientRole", Namespace="urn:hl7-org:v3")]
			public PatientRole PatientRole { get; set; }
			[XmlElement(ElementName="typeCode")]
			public string TypeCode { get; set; }
		}
		
		public class SubjectOuter {
			[XmlElement(ElementName="PdsUpdateRequest", Namespace="urn:hl7-org:v3")]
			public PdsUpdateRequest PdsUpdateRequest { get; set; }
		}
	
		public class PdsUpdateRequest {
			[XmlElement(ElementName="code", Namespace="urn:hl7-org:v3")]
			public NominatedPharmacyTypes.CodeElement Code { get; set; }
			[XmlElement(ElementName="pertinentInformation", Namespace="urn:hl7-org:v3")]
			public PertinentInformation PertinentInformation { get; set; }
			[XmlElement(ElementName="subject", Namespace="urn:hl7-org:v3")]
			public Subject Subject { get; set; }
			[XmlAttribute(AttributeName="classCode")]
			public string ClassCode { get; set; }
			[XmlAttribute(AttributeName="moodCode")]
			public string MoodCode { get; set; }
		}

		[XmlRoot(ElementName="ControlActEvent", Namespace = "urn:hl7-org:v3")]
		public class ControlActEvent {
			[XmlElement(ElementName="code", Namespace="urn:hl7-org:v3")]
			public NominatedPharmacyTypes.CodeElement Code { get; set; }
			[XmlElement(ElementName="author1", Namespace="urn:hl7-org:v3")]
			public NominatedPharmacyTypes.Author1 Author1 { get; set; }
			[XmlElement(ElementName="subject", Namespace="urn:hl7-org:v3")]
			public SubjectOuter SubjectOuter { get; set; }
			[XmlAttribute(AttributeName="xmlns")]
			public string Xmlns { get; set; }
			[XmlAttribute(AttributeName="xsi")]
			public string Xsi { get; set; }
			[XmlAttribute(AttributeName="schemaLocation")]
			public string SchemaLocation { get; set; }
			[XmlAttribute(AttributeName="classCode")]
			public string ClassCode { get; set; }
			[XmlAttribute(AttributeName="moodCode")]
			public string MoodCode { get; set; }
		}

		[XmlRoot(ElementName="PRPA_IN000203UK03", IsNullable = false)]
		public class PRPAIN000203UK03 {
			[XmlElement(ElementName="id")]
			public NominatedPharmacyTypes.Id Id { get; set; }
			[XmlElement(ElementName="creationTime")]
			public NominatedPharmacyTypes.CreationTime CreationTime { get; set; }
			[XmlElement(ElementName="versionCode")]
			public NominatedPharmacyTypes.VersionCode VersionCode { get; set; }
			[XmlElement(ElementName="interactionId")]
			public NominatedPharmacyTypes.InteractionId InteractionId { get; set; }
			[XmlElement(ElementName="processingCode")]
			public NominatedPharmacyTypes.ProcessingCode ProcessingCode { get; set; }
			[XmlElement(ElementName="processingModeCode")]
			public NominatedPharmacyTypes.ProcessingModeCode ProcessingModeCode { get; set; }
			[XmlElement(ElementName="acceptAckCode")]
			public NominatedPharmacyTypes.AcceptAckCode AcceptAckCode { get; set; }
			[XmlElement(ElementName="communicationFunctionRcv")]
			public NominatedPharmacyTypes.CommunicationFunctionRcv CommunicationFunctionRcv { get; set; }
			[XmlElement(ElementName="communicationFunctionSnd")]
			public NominatedPharmacyTypes.CommunicationFunctionSnd CommunicationFunctionSnd { get; set; }
			[XmlElement(ElementName="ControlActEvent")]
			public ControlActEvent ControlActEvent { get; set; }
			[XmlAttribute(AttributeName = "xmlns")]
			public string Xmlns { get; set; }
		}
    }
}