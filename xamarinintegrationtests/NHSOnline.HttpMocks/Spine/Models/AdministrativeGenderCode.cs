using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class AdministrativeGenderCode
    {
        [XmlAttribute(AttributeName="code")]
        public string? Code { get; set; }
    }
}