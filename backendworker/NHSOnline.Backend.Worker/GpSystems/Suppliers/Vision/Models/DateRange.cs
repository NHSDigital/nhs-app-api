using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class DateRange
    {
        [XmlElement(ElementName = "from", DataType="date")]
        public DateTime From { get; set; }
        
        [XmlElement(ElementName = "to", DataType="date")]
        public DateTime To { get; set; }
    }
}