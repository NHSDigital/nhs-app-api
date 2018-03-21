using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Suppliers.Emis.Models
{
    public class SessionsPostResponse
    {
        public string SessionId { get; set; }
        public IEnumerable<UserPatientLink> UserPatientLinks { get; set; }
    }
}