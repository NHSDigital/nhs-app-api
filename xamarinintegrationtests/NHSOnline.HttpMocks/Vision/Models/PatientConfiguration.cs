using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    public sealed class PatientConfiguration
    {
        [XmlElement(ElementName = "account", Namespace = "urn:vision")]
        public Account? Account { get; set; }

        [XmlElement(ElementName = "appointments", Namespace = "urn:vision")]
        public AppointmentsConfiguration? Appointments { get; set; }

        [XmlElement(ElementName = "prescriptions", Namespace = "urn:vision")]
        public PrescriptionsConfiguration? Prescriptions { get; set; }
    }
}