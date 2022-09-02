using System;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    [Serializable]
    public sealed class NationalId
    {
        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlText]
        public string? Value { get; set; }
    }
}