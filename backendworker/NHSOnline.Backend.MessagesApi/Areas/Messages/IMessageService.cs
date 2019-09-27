using System.Threading.Tasks;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface IMessageService
    {
        Task<MessageResult> Send(AddMessageRequest addMessageRequest, string nhsLoginId); 
    }
}