using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    [SuppressMessage("Microsoft.Naming", "CA1716", Justification = "Deliberately matching the name specified by the GPSS")]
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