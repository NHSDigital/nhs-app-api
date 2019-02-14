using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    [XmlInclude(typeof(PatientDataRequest))]
    public class PatientDataRequest
    {
        [XmlElement(ElementName = "sender", Namespace = "urn:vision")]
        public Sender Sender { get; set; }
        [XmlElement(ElementName = "practiceIdentifier", Namespace = "urn:vision")]
        public string PracticeIdentifier { get; set; }
        [XmlElement(ElementName = "patientIdentifier", Namespace = "urn:vision")]
        public string PatientIdentifier { get; set; }
        [XmlElement(ElementName = "view", Namespace = "urn:vision")]
        public string View { get; set; }
        [XmlElement(ElementName = "responseFormat", Namespace = "urn:vision")]
        public string ResponseFormat { get; set; }
    }
}
