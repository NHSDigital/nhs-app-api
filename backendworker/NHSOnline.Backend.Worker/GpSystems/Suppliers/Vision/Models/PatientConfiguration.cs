using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    public class PatientConfiguration
    {
        [XmlElement(ElementName = "account", Namespace = "urn:vision")]
        public Account Account { get; set; }

        [XmlElement(ElementName = "appointments", Namespace = "urn:vision")]
        public AppointmentsConfiguration Appointments { get; set; }
        
        [XmlElement(ElementName = "prescriptions", Namespace = "urn:vision")]
        public PrescriptionsConfiguration Prescriptions { get; set; }

        [XmlElement(ElementName = "references", Namespace = "urn:vision")]
        public PatientReferences References { get; set; }

        public IEnumerable<PatientNhsNumber> ExtractNhsNumbers()
        {
            var nhsNumbers = Account.PatientNumbers
                .Select(x => new PatientNhsNumber
                {
                    NhsNumber = x.Number.FormatToNhsNumber()
                });

            return nhsNumbers;
        }
    }
}
