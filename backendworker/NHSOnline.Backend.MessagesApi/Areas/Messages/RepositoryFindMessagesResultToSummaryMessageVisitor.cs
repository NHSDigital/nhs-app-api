using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    internal class RepositoryFindMessagesResultToSummaryMessageVisitor : IRepositoryFindResultVisitor<UserMessage, MessagesResult>
    {
        private readonly IMapper<List<SummaryMessage>, MessagesResponse> _mapper;

        public RepositoryFindMessagesResultToSummaryMessageVisitor(IMapper<List<SummaryMessage>, MessagesResponse> mapper)
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
                        SentTime = m.SentTime
                    })
                    .First()
                )
                .ToList();

            var response = _mapper.Map(summaryMessages);

            if (response.Any())
            {
                return new MessagesResult.Some(response);
            }

            return new MessagesResult.None();
        }
    }
}