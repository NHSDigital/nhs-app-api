using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class ProcessingModeCode
    {
        [XmlAttribute(AttributeName="code")]
        public string? Code { get; set; }
    }
}