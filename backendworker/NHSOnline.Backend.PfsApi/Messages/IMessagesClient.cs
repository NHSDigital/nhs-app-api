using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.Messages.Models;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public interface IMessagesClient
    {
        Task<MessagesResponse> Post(AddMessageRequest messageRequest);
    }
}
