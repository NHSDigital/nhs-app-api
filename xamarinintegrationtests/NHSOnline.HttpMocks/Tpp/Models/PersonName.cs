using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public sealed class PersonName
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }
}