using System.Threading.Tasks;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Repository
{
    public interface IMessageRepository
    {
        Task Create(UserMessage userMessage);
    }
}