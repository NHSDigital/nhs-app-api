using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis.Models
{
    public class SessionsResponse
    {
        public string SessionId { get; set; }
        public ApplicationLinkLevel ApplicationLinkLevel { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }
        public DateTime LastAccessTime { get; set; }
        public IList<UserPatientLink> UserPatientLinks { get; set; }
    }
}

