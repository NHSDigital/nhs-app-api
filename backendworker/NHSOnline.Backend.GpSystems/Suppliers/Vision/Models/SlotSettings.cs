using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class SlotSettings
    {
        [XmlElement(ElementName = "bookingReason", Namespace = "urn:vision")]
        public BookingReason BookingReason { get; set; }

        [XmlArray(ElementName = "cancellationReasons", Namespace = "urn:vision")]
        public List<Reason> CancellationReasons { get; set; }

        [XmlElement(ElementName = "cancellationCutoffMinutes", Namespace = "urn:vision")]
        public int CancellationCutOffMinutes { get; set; }
    }
}