using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientPracticeMessaging
{
    [FakeGpAreaBehaviour(Behaviour.BadGateway)]
    public class BadGatewayPatientPracticeMessagingBehaviour : IPatientPracticeMessagingAreaBehaviour
    {
        public Task<GetPatientMessagesResult> GetMessages(
            GpUserSession gpUserSession) =>
            Task.FromResult<GetPatientMessagesResult>(new GetPatientMessagesResult.BadGateway());

        public Task<GetPatientMessageResult> GetMessageDetails(string messageId, GpUserSession gpUserSession) =>
            Task.FromResult<GetPatientMessageResult>(new GetPatientMessageResult.BadGateway());

        public Task<PostPatientMessageResult> SendMessage(
            GpUserSession gpUserSession, CreatePatientMessage message) =>
            Task.FromResult<PostPatientMessageResult>(new PostPatientMessageResult.BadGateway());

        public Task<DeletePatientMessageResult> DeleteMessage(GpUserSession gpUserSession, string messageId) =>
            Task.FromResult<DeletePatientMessageResult>(new DeletePatientMessageResult.BadGateway());

        public Task<PutPatientMessageReadStatusResult> UpdateMessageMessageReadStatus(GpUserSession gpUserSession,
            UpdateMessageReadStatusRequestBody updateRequest)
            => Task.FromResult<PutPatientMessageReadStatusResult>(new PutPatientMessageReadStatusResult.BadGateway());

        public Task<GetPatientMessageRecipientsResult> GetMessageRecipients() =>
            Task.FromResult<GetPatientMessageRecipientsResult>(new GetPatientMessageRecipientsResult.Success(GetRecipients()));

        private static PatientPracticeMessageRecipients GetRecipients()
        {
            return new PatientPracticeMessageRecipients
            {
                MessageRecipients = new List<MessageRecipient>
                {
                    new MessageRecipient
                    {
                        Name = "Test Recipient",
                        RecipientIdentifier = "123"
                    },
                    new MessageRecipient
                    {
                        Name = "ONLINE, NHS (Dr)",
                        RecipientIdentifier = "456"
                    }
                },
                HasErrored = false
            };
        }
    }
}