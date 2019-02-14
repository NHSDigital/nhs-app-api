using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses
{
    [XmlRoot(ElementName = "eligibleRepeats", Namespace = "urn:vision")]
    public class EligibleRepeats
    {
        [XmlElement(ElementName = "settings", Namespace = "urn:vision")]
        public CourseSettings Settings { get; set; }

        [XmlElement(ElementName = "repeat", Namespace = "urn:vision")]
        public List<Repeat> Repeats { get; set; } = new List<Repeat>();
    }
}