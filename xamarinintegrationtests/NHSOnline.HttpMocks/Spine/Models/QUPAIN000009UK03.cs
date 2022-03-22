using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class QUPAIN000009UK03
    {
        [XmlElement(ElementName="id", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Id? Id { get; set; }

        [XmlElement(ElementName="creationTime", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public CreationTime? CreationTime { get; set; }

        [XmlElement(ElementName="versionCode", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public VersionCode? VersionCode { get; set; }

        [XmlElement(ElementName="interactionId", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public InteractionId? InteractionId { get; set; }

        [XmlElement(ElementName="processingCode", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public ProcessingCode? ProcessingCode { get; set; }

        [XmlElement(ElementName="processingModeCode", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public ProcessingModeCode? ProcessingModeCode { get; set; }

        [XmlElement(ElementName="acceptAckCode", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public AcceptAckCode? AcceptAckCode { get; set; }

        [XmlElement(ElementName="acknowledgement", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Acknowledgement? Acknowledgement { get; set; }

        [XmlElement(ElementName="communicationFunctionRcv", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public CommunicationFunctionRcv? CommunicationFunctionRcv { get; set; }

        [XmlElement(ElementName="communicationFunctionSnd", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public CommunicationFunctionSnd? CommunicationFunctionSnd{ get; set; }

        [XmlElement(ElementName="ControlActEvent", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public ControlActEvent? ControlActEvent { get; set; }
    }
}