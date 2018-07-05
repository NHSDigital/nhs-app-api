using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments
{
    public class ListSlots : AbstractTppRequestModel
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("startDate")]
        public DateTime StartDate { get; set; }

        [XmlAttribute("numberOfDays")]
        public int NumberOfDays { get; set; }

        [XmlIgnore]
        public override string RequestType => "ListSlots";
    }
}
