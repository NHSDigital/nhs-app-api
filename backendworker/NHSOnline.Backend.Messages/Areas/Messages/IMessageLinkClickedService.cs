using System.Threading.Tasks;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface IMessageLinkClickedService
    {
        Task<MessageLinkClickedResult> LogLinkClicked(string nhsLoginId, MessageLink messageLink);
    }
}
