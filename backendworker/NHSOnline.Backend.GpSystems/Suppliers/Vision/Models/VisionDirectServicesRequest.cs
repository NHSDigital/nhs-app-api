using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "visionRequest", Namespace = "urn:vision")]
    public class VisionDirectServicesRequest
    {
        [XmlElement(ElementName = "credentials", Namespace = "urn:vision")]
        public Credentials Credentials { get; set; }

        [XmlElement(ElementName = "opsReference", Namespace = "urn:vision")]
        public OpsReference OpsReference { get; set; }

        [XmlElement(ElementName = "vos", Namespace = "urn:vision")]
        public Vos Vos { get; set; }

        public VisionDirectServicesRequest(string rosuAccountId, string apiKey,  string providerId, string patientId) : this(rosuAccountId, apiKey, providerId)
        {
            Vos = new Vos
            {
                PatientId = patientId
            };
        }

        public VisionDirectServicesRequest(string rosuAccountId, string apiKey,  string providerId)
        {
            Credentials = new Credentials
            {
                ApiKey = apiKey,
                RosuAccountId = rosuAccountId
            };
            OpsReference = new OpsReference
            {
                // AccountId and provider will always be the same value
                AccountId = providerId,
                Provider = providerId
            };
        }

        public VisionDirectServicesRequest() { }
    }
}
