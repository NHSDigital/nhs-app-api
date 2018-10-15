using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public class NewPrescriptionRepeat
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
