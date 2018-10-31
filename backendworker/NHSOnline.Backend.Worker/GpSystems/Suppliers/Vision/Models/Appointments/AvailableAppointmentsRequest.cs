using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments
{
    public class AvailableAppointmentsRequest
    {
        [XmlElement(ElementName = "patientId", Namespace = "urn:vision")]
        public string PatientId { get; set; }

        [XmlElement(ElementName = "pageInformationRequired", Namespace = "urn:vision")]
        public bool PageInformationRequired { get; set; } = false;
        
        [XmlElement(ElementName = "page", Namespace = "urn:vision")]
        public Page Page { get; set; }
        
        [XmlElement(ElementName = "owners", Namespace = "urn:vision")]
        public List<OwnerFilter> Owners { get; set; } = new List<OwnerFilter>{new OwnerFilter()}; //TODO: NHSO-2816
        
        [XmlElement(ElementName = "locations", Namespace = "urn:vision")]
        public List<LocationFilter> Locations { get; set; } = new List<LocationFilter>{new LocationFilter()}; //TODO: NHSO-2816
        
        [XmlElement(ElementName = "dateRange", Namespace = "urn:vision")]
        public DateRange DateRange { get; set; }

        [XmlElement(ElementName = "am", Namespace = "urn:vision")]
        public bool Am { get; set; } = true;
        
        [XmlElement(ElementName = "pm", Namespace = "urn:vision")]
        public bool Pm { get; set; } = true;
    }
}