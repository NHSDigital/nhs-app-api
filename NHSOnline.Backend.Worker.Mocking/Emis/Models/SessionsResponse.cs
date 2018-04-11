using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class SessionsResponse
    {
        public string SessionId { get; }
        public ApplicationLinkLevel ApplicationLinkLevel { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string Title { get; }
        public DateTime LastAccessTime { get; }
        public IList<UserPatientLink> UserPatientLinks { get; }

        public SessionsResponse(string sessionId, string title, string firstName, string surname, string userPatientLinkToken, string odsCode, AssociationType associationType)
        {
            SessionId = sessionId;
            ApplicationLinkLevel = ApplicationLinkLevel.Linked;
            FirstName = firstName;
            Surname = surname;
            Title = title;
            LastAccessTime = DateTime.Now;
            UserPatientLinks = new[]
            {
                new UserPatientLink(title, firstName, surname, userPatientLinkToken, odsCode, associationType)
            };
        }
    }
}

