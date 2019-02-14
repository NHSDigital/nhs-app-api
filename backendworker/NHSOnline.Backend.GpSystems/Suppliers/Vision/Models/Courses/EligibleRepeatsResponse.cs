using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses
{
    public class EligibleRepeatsResponse
    {
        [XmlElement(ElementName = "eligibleRepeats", Namespace = "urn:vision")]
        public EligibleRepeats EligibleRepeats { get; set; }
    }
}