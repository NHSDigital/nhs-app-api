using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface IMessageService
    {
        Task<MessageResult> Send(AddMessageRequest addMessageRequest, string nhsLoginId);

        Task<MessagesResult> GetMessages(AccessToken accessToken, string sender);

        Task<MessagesResult> GetSummaryMessages(AccessToken accessToken);
    }
}