using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    public abstract class AbstractTppRequestModel : ITppRequest
    {
        [XmlAttribute("apiVersion")]
        public string ApiVersion { get; set; }
        
        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }
        
        [XmlAttribute("unitId")]
        public string UnitId { get; set; }

        [XmlIgnore]
        public abstract string RequestType { get; }
    }
}