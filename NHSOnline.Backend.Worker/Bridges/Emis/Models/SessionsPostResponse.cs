using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models
{
    public class SessionsPostResponse
    {
        public string SessionId { get; set; }
        public IEnumerable<UserPatientLink> UserPatientLinks { get; set; }
    }
}