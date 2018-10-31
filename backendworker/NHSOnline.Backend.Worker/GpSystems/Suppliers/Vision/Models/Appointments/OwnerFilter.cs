using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class OwnerFilter
    {
        [XmlElement(ElementName = "owner")]
        public string Owner { get; set; } = "ALL"; //TODO: NHSO-2816
    }
}