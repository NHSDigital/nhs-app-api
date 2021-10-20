using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public interface IIntroMessagesService
    {
        Task<MessagesResult> SendIntroductoryMessage(string nhsLoginId);
    }
}
