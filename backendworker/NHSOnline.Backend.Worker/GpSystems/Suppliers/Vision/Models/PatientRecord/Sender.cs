using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    [XmlInclude(typeof(Sender))]
    public class Sender
    {
        [XmlElement(ElementName = "name", Namespace = "urn:vision")]
        public SenderName Name { get; set; }
    }
}
