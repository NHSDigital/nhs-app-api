using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [Serializable]
    public class Slot
    {
        [XmlAttribute("startDate")]
        public string StartDate {get; set;}

        [XmlAttribute("endDate")]
        public string EndDate { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}
