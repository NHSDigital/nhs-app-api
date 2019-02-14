using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class LogoffReply
    {
        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }
    }
}