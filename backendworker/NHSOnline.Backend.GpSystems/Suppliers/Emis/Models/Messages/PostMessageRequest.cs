using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages
{
    public class PostMessageRequest
    {

        public PostMessageRequest(string userPatientLinkToken, CreatePatientMessage message)
        {
            UserPatientLinkToken = userPatientLinkToken;
            Subject = message.Subject;
            MessageBody = message.MessageBody;
            Recipients = new List<string> { message.Recipient };
        }

        public string UserPatientLinkToken { get; }
        public string Subject { get; }
        public string MessageBody { get; }
        public List<string> Recipients { get; }
    }
}