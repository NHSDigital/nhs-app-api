using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class Registration
    {
        [XmlElement("PatientAccess")]
        public List<PatientAccess> PatientAccess { get; set; }
    }
}
