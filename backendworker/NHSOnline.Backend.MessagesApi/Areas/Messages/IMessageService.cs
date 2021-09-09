using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface IMessageService
    {
        Task<AddMessageResult> Send(AddMessageRequest addMessageRequest, string nhsLoginId);

        Task<MessagesResult> GetMessage(AccessToken accessToken, string messageId);
        Task<MessagesResult> GetMessages(AccessToken accessToken, string sender);

        Task<MessagesResult> GetSummaryMessages(AccessToken accessToken);
        
        Task<MessagePatchResult> UpdateMessage(JsonPatchDocument<Message> messagePatchDocument, AccessToken accessToken, string messageId);
    }
}