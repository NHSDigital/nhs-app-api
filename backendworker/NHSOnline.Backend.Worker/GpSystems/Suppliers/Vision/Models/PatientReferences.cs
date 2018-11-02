using System.Collections.Generic;
using System.Xml.Serialization;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class PatientReferences
    {
        [XmlElement(ElementName = "owner", Namespace = "urn:vision")]
        public List<Owner> Owners { get; set; }

        [XmlElement(ElementName = "location", Namespace = "urn:vision")]
        public List<Location> Locations { get; set; }
    }
}
