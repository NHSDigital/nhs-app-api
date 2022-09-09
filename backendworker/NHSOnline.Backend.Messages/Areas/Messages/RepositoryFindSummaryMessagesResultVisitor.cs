using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    internal class RepositoryFindSummaryMessagesResultVisitor : IRepositoryFindResultVisitor<UserMessage, MessagesResult>
    {
        private readonly IMapper<List<SummaryMessage>, MessagesResponse> _mapper;

        public RepositoryFindSummaryMessagesResultVisitor(IMapper<List<SummaryMessage>, MessagesResponse> mapper)
        {
            _mapper = mapper;
        }

        public MessagesResult Visit(RepositoryFindResult<UserMessage>.NotFound result)
        {
            return new MessagesResult.None();
        }

        public MessagesResult Visit(RepositoryFindResult<UserMessage>.RepositoryError result)
        {
            return new MessagesResult.BadGateway();
        }

        public MessagesResult Visit(RepositoryFindResult<UserMessage>.Found result)
        {
            var summaryMessages = result.Records.OrderByDescending(x => x.SentTime)
                .GroupBy(k => k.Sender)
                .Select(g => g.Select(m => new SummaryMessage
                    {
                        UnreadCount = g.Count(v => !v.ReadTime.HasValue),
                        Id = m.Id,
                        NhsLoginId = m.NhsLoginId,
                        Sender = m.Sender,
                        Version = m.Version,
                        Body = m.Body,
                        ReadTime = m.ReadTime,
                        SentTime = m.SentTime,
                        Reply = m.Reply
                    })
                    .First()
                )
                .ToList();

            var response = _mapper.Map(summaryMessages);

            if (response.SenderMessages.Any())
            {
                return new MessagesResult.Found(response);
            }

            return new MessagesResult.None();
        }
    }
}