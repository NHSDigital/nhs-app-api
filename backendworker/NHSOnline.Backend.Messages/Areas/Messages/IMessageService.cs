using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface IMessageService
    {
        Task<AddMessageResult> Send(AddMessageRequest addMessageRequest, string nhsLoginId);
        Task<MessagesResult> GetMessage(AccessToken accessToken, string messageId);
        Task<MessagesResult> GetMessagesBySenderId(AccessToken accessToken, string senderId);
        Task<MessagesMetadataResult> GetMessagesMetadata(AccessToken accessToken);
        Task<UnreadMessageCountResult> GetUnreadMessageCount(string nhsLoginId);
        Task<MessagesResult> GetSummaryMessages(AccessToken accessToken);
        Task<MessagePatchResult> UpdateMessage(JsonPatchDocument<Message> messagePatchDocument, AccessToken accessToken, string messageId);
        Task<MessagePatchResult> UpdateMessage(JsonPatchDocument<Message> messagePatchDocument, string nhsLoginId, string messageId);
        Task<UserSendersResult> GetSenders(AccessToken accessToken);
    }
}