using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    [XmlRoot(ElementName="gender", Namespace="urn:vision")]
    public class Gender 
    {   
        [XmlText]
        public string Text { get; set; }
    }
}