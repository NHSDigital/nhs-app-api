using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments
{
    public class Appointment
    {
        [XmlAttribute("apptId")]
        public string ApptId { get; set; }

        [XmlAttribute("startDate")]
        public string StartDate { get; set; }

        [XmlAttribute("endDate")]
        public string EndDate { get; set; }

        [XmlAttribute("details")]
        public string Details { get; set; }

        [XmlAttribute("siteName")]
        public string SiteName { get; set; }

        [XmlAttribute("address")]
        public string Address { get; set; }
    }
}