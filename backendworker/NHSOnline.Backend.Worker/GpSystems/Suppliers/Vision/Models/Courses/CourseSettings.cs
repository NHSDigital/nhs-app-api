using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Courses
{
    public class CourseSettings
    {
        [XmlElement(ElementName = "allowFreetext", Namespace = "urn:vision")]
        public bool AllowFreeText { get; set; }
    }
}