using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public class NewPrescriptionRepeat
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
