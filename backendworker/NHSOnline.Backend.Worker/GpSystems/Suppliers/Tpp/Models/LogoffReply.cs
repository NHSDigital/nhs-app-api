using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class LogoffReply
    {
        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }
    }
}