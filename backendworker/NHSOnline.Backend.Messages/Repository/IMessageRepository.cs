using System.Threading.Tasks;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.Repository
{
    public interface IMessageRepository
    {
        Task<RepositoryCreateResult<UserMessage>> Create(UserMessage userMessage);
        Task<RepositoryFindResult<UserMessage>> FindMessagesFromSenderByName(string nhsLoginId, string sender);
        Task<RepositoryFindResult<UserMessage>> FindMessagesFromSenderById(string nhsLoginId, string senderId);
        Task<RepositoryFindResult<UserMessage>> FindAllForUser(string nhsLoginId);
        Task<RepositoryFindResult<UserMessage>> FindAllForUserV1(string nhsLoginId);
        Task<RepositoryFindResult<UserMessage>> FindMessage(string nhsLoginId, string messageId);
        Task<RepositoryUpdateResult<UserMessage>> UpdateOne(string nhsLoginId, string messageId,
            UpdateRecordBuilder<UserMessage> updates);
    }
}