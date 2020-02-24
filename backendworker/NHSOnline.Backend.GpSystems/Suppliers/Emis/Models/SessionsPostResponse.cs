using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class SessionsPostResponse
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string SessionId { get; set; }
        public IEnumerable<UserPatientLink> UserPatientLinks { get; set; } = new List<UserPatientLink>();

        public string ExtractUserPatientLinkToken()
        {
            return UserPatientLinks
                ?.FirstOrDefault(x => x.AssociationType == AssociationType.Self)
                ?.UserPatientLinkToken;
        }
        
        public IEnumerable<UserPatientLink> ExtractLinkedPatients()
        {
            return UserPatientLinks
                .Where(x => x.AssociationType == AssociationType.Proxy);
        }

        public UserPatientLink ExtractSelfPatient()
        {
            return UserPatientLinks
                ?.FirstOrDefault(x => x.AssociationType == AssociationType.Self);
        }
    }
}
