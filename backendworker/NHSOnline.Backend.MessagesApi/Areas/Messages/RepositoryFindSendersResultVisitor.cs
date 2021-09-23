using System.Linq;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    internal class RepositoryFindSendersResultVisitor : IRepositoryFindResultVisitor<UserMessage, SendersResult>
    {
        public SendersResult Visit(RepositoryFindResult<UserMessage>.NotFound result)
        {
            return new SendersResult.None();
        }

        public SendersResult Visit(RepositoryFindResult<UserMessage>.Found result)
        {
            var senders = result.Records
                .OrderByDescending(x => x.SentTime)
                .GroupBy(k => k.Sender)
                .Select(g => new Sender
                {
                    Name = g.Key,
                    UnreadCount = g.Count(v => !v.ReadTime.HasValue)
                })
                .ToList();

            if (senders.Any())
            {
                return new SendersResult.Found(new SendersResponse { Senders = senders });
            }

            return new SendersResult.None();
        }

        public SendersResult Visit(RepositoryFindResult<UserMessage>.RepositoryError result)
        {
            return new SendersResult.BadGateway();
        }
    }
}