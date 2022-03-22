using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class ValidTime
    {
        [XmlElement(ElementName="low", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public Low? Low { get; set; }
    }
}