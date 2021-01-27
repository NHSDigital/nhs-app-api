using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
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
        public List<Person> People { get; } = new List<Person>();

        public Registration? Registration { get; set; }
    }
}