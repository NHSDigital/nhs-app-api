using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class Patient
    {
        [XmlElement(ElementName = "clinical")]
        public List<Clinical> Clinicals { get; set; }
    }
}
