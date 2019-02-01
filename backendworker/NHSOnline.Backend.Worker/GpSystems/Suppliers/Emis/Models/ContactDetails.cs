using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class ContactDetails
    {
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        
        public PatientTelephoneNumber[] GetTelephoneArray()
        {
            return new[] { TelephoneNumber, MobileNumber }
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new PatientTelephoneNumber { TelephoneNumber = x.Trim() })
                .ToArray();
        }

    }
}