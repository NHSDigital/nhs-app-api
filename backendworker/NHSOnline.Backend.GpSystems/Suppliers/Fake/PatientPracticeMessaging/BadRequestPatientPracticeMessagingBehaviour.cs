using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientPracticeMessaging
{
    [FakeGpAreaBehaviour(Behaviour.BadRequest)]
    public class BadRequestPatientPracticeMessagingBehaviour : IPatientPracticeMessagingAreaBehaviour
    {
        public Task<GetPatientMessagesResult> GetMessages(
            GpUserSession gpUserSession)
            => Task.FromResult<GetPatientMessagesResult>(new GetPatientMessagesResult.Success(GetFakePatientMessagesResponse()));

        public Task<GetPatientMessageResult> GetMessageDetails(string messageId, GpUserSession gpUserSession) =>
            Task.FromResult<GetPatientMessageResult>(new GetPatientMessageResult.BadRequest());

        public Task<PostPatientMessageResult> SendMessage(
            GpUserSession gpUserSession, CreatePatientMessage message) =>
            Task.FromResult<PostPatientMessageResult>(new PostPatientMessageResult.BadRequest());

        public Task<DeletePatientMessageResult> DeleteMessage(GpUserSession gpUserSession, string messageId) =>
            Task.FromResult<DeletePatientMessageResult>(new DeletePatientMessageResult.BadRequest());

        public Task<PutPatientMessageReadStatusResult> UpdateMessageMessageReadStatus(GpUserSession gpUserSession,
            UpdateMessageReadStatusRequestBody updateRequest)
            => Task.FromResult<PutPatientMessageReadStatusResult>(new PutPatientMessageReadStatusResult.BadRequest());


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

        private static GetPatientMessagesResponse GetFakePatientMessagesResponse()
        {
            return new GetPatientMessagesResponse
            {
                MessageSummaries = new List<PatientMessageSummary>
                {
                    new PatientMessageSummary
                    {
                        Content = null,
                        IsUnread = false,
                        LastMessageDateTime = "2021-10-25T17:00:03.237",
                        MessageId = "734",
                        OutboundMessage = true,
                        Recipient = "ONLINE, NHS (Dr)",
                        ReplyCount = 0,
                        Subject = "Test",
                        UnreadReplyInfo = new UnreadReplyInfo { Present = false, Count = null}
                    },
                    new PatientMessageSummary
                    {
                        Content = null,
                        IsUnread = false,
                        LastMessageDateTime = "2021-10-26T17:00:03.237",
                        MessageId = "735",
                        OutboundMessage = true,
                        Recipient = "ONLINE, NHS (Dr)",
                        ReplyCount = 0,
                        Subject = "Test 1",
                        UnreadReplyInfo = new UnreadReplyInfo { Present = false, Count = null}
                    }
                }
            };
        }
    }
}