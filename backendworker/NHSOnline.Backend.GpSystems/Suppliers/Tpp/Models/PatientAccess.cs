using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class PatientAccess
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        public SiteDetails SiteDetails { get; set; }
    }
}
