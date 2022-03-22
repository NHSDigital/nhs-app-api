using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class From
    {
        [XmlElement(ElementName="Address", Namespace="http://schemas.xmlsoap.org/ws/2004/08/addressing", IsNullable = true)]
        public string? Address { get; set; }
    }
}