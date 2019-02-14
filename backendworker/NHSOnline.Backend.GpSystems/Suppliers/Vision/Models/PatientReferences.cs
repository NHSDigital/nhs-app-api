using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class PatientReferences
    {
        [XmlElement(ElementName = "owner", Namespace = "urn:vision")]
        public List<Owner> Owners { get; set; }

        [XmlElement(ElementName = "location", Namespace = "urn:vision")]
        public List<Location> Locations { get; set; }
    }
}
