using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class PatientConfigurationResponse
    {
        [XmlElement(ElementName = "configuration", Namespace = "urn:vision")]
        public PatientConfiguration Configuration { get; set; }
    }
}
