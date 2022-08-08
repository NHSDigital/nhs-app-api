using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.Repository
{
    public interface IMessageRepository
    {
        Task<RepositoryCountResult> CountUnreadMessages(string nhsLoginId);
        Task<RepositoryCreateResult<UserMessage>> Create(UserMessage userMessage);
        Task<RepositoryFindResult<UserMessage>> FindMessagesFromSenderByName(string nhsLoginId, string sender);
        Task<RepositoryFindResult<UserMessage>> FindMessagesFromSenderById(string nhsLoginId, string senderId);
        Task<RepositoryFindResult<UserMessage>> FindAllForUser(string nhsLoginId);
        Task<RepositoryFindResult<UserMessage>> FindAllForUserV1(string nhsLoginId);
        Task<RepositoryFindResult<UserMessage>> FindMessage(string nhsLoginId, string messageId);
        Task<RepositoryUpdateResult<UserMessage>> UpdateOne(string nhsLoginId, string messageId,
            (List<Expression<Func<UserMessage, bool>>> filters, UpdateRecordBuilder<UserMessage> updates) filtersAndUpdates);
    }
}