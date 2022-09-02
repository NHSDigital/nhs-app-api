using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    [Serializable]
    public class AuthenticateReply
    {
        [XmlAttribute("patientId")]
        public string? PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string? OnlineUserId { get; set; }

        public User? User { get; set; }

        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }

        [XmlIgnore]
        public string? Suid { get; set; }

        [XmlElement("Person")]
        public Collection<Person> People { get; } = new Collection<Person>();

        public Registration? Registration { get; set; }
    }
}