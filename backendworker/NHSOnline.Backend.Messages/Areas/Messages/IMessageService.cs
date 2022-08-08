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

        Task<MessagesResult> GetMessagesBySender(AccessToken accessToken, string sender);

        Task<MessagesResult> GetMessagesBySenderId(AccessToken accessToken, string senderId);

        Task<MessagesMetadataResult> GetMessagesMetadata(AccessToken accessToken);

        Task<MessagesResult> GetSummaryMessages(AccessToken accessToken);

        Task<MessagePatchResult> UpdateMessage(JsonPatchDocument<Message> messagePatchDocument, AccessToken accessToken, string messageId);

        Task<UserSendersResult> GetSendersV2(AccessToken accessToken);

        Task<UserSendersResult> GetSenders(AccessToken accessToken);
    }
}