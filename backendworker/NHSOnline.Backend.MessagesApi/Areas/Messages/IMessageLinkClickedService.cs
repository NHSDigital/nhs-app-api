using System.Threading.Tasks;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface IMessageLinkClickedService
    {
        Task<MessageLinkClickedResult> LogLinkClicked(string nhsLoginId, MessageLink messageLink);
    }
}
