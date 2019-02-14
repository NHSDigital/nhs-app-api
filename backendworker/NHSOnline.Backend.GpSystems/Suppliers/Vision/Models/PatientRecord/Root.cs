using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    [XmlRoot(ElementName = "root")]
    public class Root
    {
        [XmlElement(ElementName = "patient")]
        public Patient Patient { get; set; }
    }
}
