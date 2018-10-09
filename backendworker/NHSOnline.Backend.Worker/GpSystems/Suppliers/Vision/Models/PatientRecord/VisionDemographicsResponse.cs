using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class VisionDemographicsResponse
    {
        [XmlElement(ElementName = "demographics", Namespace = "urn:vision")]
        public VisionDemographics Demographics { get; set; }
    }
}