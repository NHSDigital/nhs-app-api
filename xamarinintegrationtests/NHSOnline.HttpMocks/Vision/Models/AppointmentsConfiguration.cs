using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    internal sealed class AppointmentsConfiguration
    {
        [XmlElement(ElementName = "enabled", Namespace = "urn:vision")]
        public bool BookingEnabled { get; set; } = true;

        [XmlArray(ElementName = "welcomeText", Namespace = "urn:vision")]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Required to be serialized")]
        public List<VisionMessage> VisionMessage { get; set; } = new List<VisionMessage>();
    }
}