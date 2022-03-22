using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class InteractionId
    {

        [XmlAttribute(AttributeName="root")]
        public string? Root { get; set; }

        [XmlAttribute(AttributeName="extension")]
        public string? Extension { get; set; }
    }
}