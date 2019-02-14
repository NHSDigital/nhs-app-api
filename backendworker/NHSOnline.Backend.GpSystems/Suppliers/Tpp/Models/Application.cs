using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class Application
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        
        [XmlAttribute("version")]
        public string Version { get; set; }
        
        [XmlAttribute("providerId")]
        public string ProviderId { get; set; }
        
        [XmlAttribute("deviceType")]
        public string DeviceType { get; set; }
    }
}