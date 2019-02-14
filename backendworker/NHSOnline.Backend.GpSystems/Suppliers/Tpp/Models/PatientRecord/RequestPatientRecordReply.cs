using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord
{
    public class RequestPatientRecordReply
    {        
        [XmlElement("Event")]
        public List<Event> Events { get; set; } 
    }
}