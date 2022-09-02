using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class BinaryDataPage
    {
        [XmlText] public string? BinaryData { get; set; }
    }
}