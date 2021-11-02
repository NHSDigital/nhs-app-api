using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientPracticeMessaging
{
    [FakeGpAreaBehaviour(Behaviour.Default)]
    public class DefaultPatientPracticeMessagingBehaviour : IPatientPracticeMessagingAreaBehaviour
    {
        public Task<GetPatientMessagesResult> GetMessages(GpUserSession gpUserSession)
            => Task.FromResult<GetPatientMessagesResult>(new GetPatientMessagesResult.Success(new GetPatientMessagesResponse()));

        public Task<GetPatientMessageResult> GetMessageDetails(string messageId, GpUserSession gpUserSession)
            => Task.FromResult<GetPatientMessageResult>(new GetPatientMessageResult.Success(new GetPatientMessageResponse()));

        public Task<PostPatientMessageResult> SendMessage(GpUserSession gpUserSession, CreatePatientMessage message)
            => Task.FromResult<PostPatientMessageResult>(new PostPatientMessageResult.Success());

        public Task<DeletePatientMessageResult> DeleteMessage(GpUserSession gpUserSession, string messageId)
            => Task.FromResult<DeletePatientMessageResult>(new DeletePatientMessageResult.Success());

        public Task<PutPatientMessageReadStatusResult> UpdateMessageMessageReadStatus(GpUserSession gpUserSession,
            UpdateMessageReadStatusRequestBody updateRequest)
            => Task.FromResult<PutPatientMessageReadStatusResult>(new PutPatientMessageReadStatusResult.Success());

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