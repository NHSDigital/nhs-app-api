using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord
{
    public class RequestPatientRecordReply
    {        
        [XmlElement("Event")]
        public List<Event> Events { get; set; } 
    }
}