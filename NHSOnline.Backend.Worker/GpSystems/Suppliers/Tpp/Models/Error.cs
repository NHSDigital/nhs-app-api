using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    public class Error
    {
        [XmlAttribute("errorCode")]
        public string ErrorCode { get; set; }
        
        [XmlAttribute("userFriendlyMessage")]
        public string UserFriendlyMessage { get; set; }

        [XmlAttribute("technicalMessage")]
        public string TechnicalMessage { get; set; }

        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }
    }
}