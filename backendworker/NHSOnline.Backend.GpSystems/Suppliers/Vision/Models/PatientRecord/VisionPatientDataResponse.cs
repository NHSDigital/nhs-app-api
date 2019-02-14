using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class VisionPatientDataResponse
    {
        [XmlElement(ElementName = "record")]
        public string Record { get; set; }
    }
}
