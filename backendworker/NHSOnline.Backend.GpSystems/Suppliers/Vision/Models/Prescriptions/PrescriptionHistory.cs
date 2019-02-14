using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public class PrescriptionHistory
    {
        [XmlElement(ElementName = "request", Namespace = "urn:vision")]
        public List<Request> Requests { get; set; } = new List<Request>();
    }
}
