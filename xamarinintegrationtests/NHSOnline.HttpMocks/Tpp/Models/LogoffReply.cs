using System;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public sealed class LogoffReply
    {
        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }
    }
}