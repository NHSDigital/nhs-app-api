using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.Repository
{
    public interface IMessageRepository
    {
        Task<RepositoryCreateResult<UserMessage>> Create(UserMessage userMessage);
        Task<RepositoryFindResult<UserMessage>> FindMessagesFromSender(string nhsLoginId, string sender);
        Task<RepositoryFindResult<UserMessage>> FindAllForUser(string nhsLoginId);
        Task<RepositoryFindResult<UserMessage>> FindMessage(string nhsLoginId, string messageId);

        Task<RepositoryUpdateResult<UserMessage>> UpdateOne(string nhsLoginId, string messageId,
            UpdateRecordBuilder<UserMessage> updates);
    }
}