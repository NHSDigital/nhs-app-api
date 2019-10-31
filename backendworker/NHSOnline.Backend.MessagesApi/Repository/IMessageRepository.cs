using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Repository
{
    public interface IMessageRepository
    {
        Task Create(UserMessage userMessage);
        Task<List<UserMessage>> Find(string nhsLoginId, string sender);
        Task<List<SummaryMessage>> Summary(string nhsLoginId);
        Task<UserMessage> FindOne(string messageId);
        Task UpdateOne(UserMessage userMessage);
    }
}