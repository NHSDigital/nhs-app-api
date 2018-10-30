using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class DateRange
    {
        [XmlElement(ElementName = "from")]
        public DateTime From { get; set; }
        
        [XmlElement(ElementName = "to")]
        public DateTime To { get; set; }
    }
}