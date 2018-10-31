using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class LocationFilter
    {
        [XmlElement(ElementName = "location")]
        public string Location { get; set; } = "1"; //TODO: NHSO-2816
    }
}