using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class SiteDetails
    {
        [XmlAttribute("unitName")]
        public string UnitName { get; set; }

        public TppAddress Address { get; set; }
    }
}
