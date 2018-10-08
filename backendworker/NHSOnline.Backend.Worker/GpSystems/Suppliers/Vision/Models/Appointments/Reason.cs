using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    [XmlType(TypeName = "reason", Namespace = "urn:vision")]
    public class Reason
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "description", Namespace = "urn:vision")]
        public List<Description> Descriptions { get; set; }
    }
}