using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class Patient
    {
        [XmlElement(ElementName = "clinical")]
        public List<Clinical> Clinicals { get; set; }
        
        [XmlElement(ElementName = "problems")]
        public List<Clinical> Problems { get; set; }
    }
}
