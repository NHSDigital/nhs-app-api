using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class VisionPatientDataResponse
    {
        [XmlElement(ElementName = "record")]
        public string Record { get; set; }
    }
}
