using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class Id
    {
        [XmlAttribute(AttributeName="root")]
        public string? Root { get; set; }

        [XmlAttribute(AttributeName="extension")]
        public string? Extension { get; set; }
    }
}