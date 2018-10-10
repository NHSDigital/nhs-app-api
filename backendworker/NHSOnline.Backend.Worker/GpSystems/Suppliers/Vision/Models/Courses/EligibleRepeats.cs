using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Courses
{
        [XmlRoot(ElementName = "eligibleRepeats", Namespace = "urn:vision")]
        public class EligibleRepeats
        {
            [XmlElement(ElementName = "repeat", Namespace = "urn:vision")]
            public List<Repeat> Repeat { get; set; }
        }
}