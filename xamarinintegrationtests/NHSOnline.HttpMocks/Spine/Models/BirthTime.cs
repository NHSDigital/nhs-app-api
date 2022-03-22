using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class BirthTime
    {
        [XmlAttribute(AttributeName="value")]
        public string? Value { get; set; }
    }
}