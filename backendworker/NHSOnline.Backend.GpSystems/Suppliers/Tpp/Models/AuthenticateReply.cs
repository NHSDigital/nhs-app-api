using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class AuthenticateReply
    {
        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }

        public User User { get; set; }

        public Registration Registration { get; set; }

        [XmlElement("Person")]
        public List<Person> People { get; set; } = new List<Person>();

        public IEnumerable<PatientNhsNumber> ExtractNhsNumbers()
        {
            var nhsNumber = User?.Person?.NationalId?.Value;
            var nhsNumbers = Enumerable.Empty<PatientNhsNumber>();

            if (nhsNumber != null)
            {
                nhsNumbers = new List<PatientNhsNumber>
                {
                    new PatientNhsNumber
                    {
                        NhsNumber = nhsNumber.FormatToNhsNumber()
                    }
                };
            }

            return nhsNumbers;
        }

        public IEnumerable<Person> ExtractLinkedPatients()
        {
            return People.Where(x =>
                x.PatientId != null && !OnlineUserId.Equals(x.PatientId, StringComparison.Ordinal));
        }

        public SiteDetails GetSiteDetails(string patientId)
        {
            return Registration.PatientAccess
                    .FirstOrDefault(x => x.PatientId == patientId)?.SiteDetails;
        }
    }
}