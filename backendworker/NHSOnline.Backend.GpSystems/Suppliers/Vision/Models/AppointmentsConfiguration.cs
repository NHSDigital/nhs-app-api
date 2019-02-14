using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class AppointmentsConfiguration
    {
        [XmlElement(ElementName = "enabled", Namespace = "urn:vision")]
        public bool BookingEnabled { get; set; } = true;

        [XmlArray(ElementName = "welcomeText", Namespace = "urn:vision")]
        public List<AppointmentsMessage> WelcomeText { get; set; } = new List<AppointmentsMessage>();
    }
}