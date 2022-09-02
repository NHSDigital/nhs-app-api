using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class RequestPatientRecordReply
    {
        [XmlElement("Event")]
        public Collection<Event>? Events { get; set; }
    }
}