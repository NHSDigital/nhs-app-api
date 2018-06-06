using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class SessionsPostResponse
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string SessionId { get; set; }
        public IEnumerable<UserPatientLink> UserPatientLinks { get; set; }
    }
}