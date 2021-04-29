using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public interface IMessagesService
    {
        Task<MessagesResult> SendIntroductoryMessage(string nhsLoginId);
    }
}
