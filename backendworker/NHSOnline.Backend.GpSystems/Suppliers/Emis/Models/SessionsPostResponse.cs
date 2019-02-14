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
        public IEnumerable<UserPatientLink> UserPatientLinks { get; set; }

        public string ExtractUserPatientLinkToken()
        {
            return UserPatientLinks
                ?.FirstOrDefault(x => x.AssociationType == AssociationType.Self)
                ?.UserPatientLinkToken;
        }
    }
}