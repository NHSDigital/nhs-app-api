using System.Linq;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    internal class RepositoryFindUserSendersResultVisitor : IRepositoryFindResultVisitor<UserMessage, UserSendersResult>
    {
        public UserSendersResult Visit(RepositoryFindResult<UserMessage>.NotFound result)
        {
            return new UserSendersResult.None();
        }

        public UserSendersResult Visit(RepositoryFindResult<UserMessage>.Found result)
        {
            var senders = result.Records
                .OrderByDescending(x => x.SentTime)
                .GroupBy(k => k.Sender)
                .Select(g => new UserSender
                {
                    Name = g.Key,
                    UnreadCount = g.Count(v => !v.ReadTime.HasValue)
                })
                .ToList();

            if (senders.Any())
            {
                return new UserSendersResult.Found(new UserSendersResponse { Senders = senders });
            }

            return new UserSendersResult.None();
        }

        public UserSendersResult Visit(RepositoryFindResult<UserMessage>.RepositoryError result)
        {
            return new UserSendersResult.BadGateway();
        }
    }
}