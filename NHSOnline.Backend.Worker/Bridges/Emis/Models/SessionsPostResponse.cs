using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models
{
    public class SessionsPostResponse
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string SessionId { get; set; }
        public IEnumerable<UserPatientLink> UserPatientLinks { get; set; }
    }
}