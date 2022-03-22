using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class CreationTime
    {
        [XmlAttribute(AttributeName="value")]
        public string? Value { get; set; }
    }
}