using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers
{
    internal class MessagesResponseMapper :
        IMapper<List<UserMessage>, MessagesResponse>,
        IMapper<List<SummaryMessage>, MessagesResponse>
    {
        private readonly ILogger<MessagesResponseMapper> _logger;

        public MessagesResponseMapper(ILogger<MessagesResponseMapper> logger)
        {
            _logger = logger;
        }

        public MessagesResponse Map(List<UserMessage> source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();

            if (!source.Any())
            {
                return new MessagesResponse();
            }

            return new MessagesResponse
            {
                new SenderMessages
                {
                    Sender = source.First().Sender,
                    UnreadCount = source.Count(m => !m.ReadTime.HasValue),
                    Messages = source.OrderBy(m => m.SentTime)
                        .Select(m => new Message
                        {
                            Id = m.Id,
                            Sender = m.Sender,
                            Version = m.Version,
                            Body = m.Body,
                            Read = m.ReadTime.HasValue,
                            SentTime = m.SentTime,
                        }).ToList()
                }
            };
        }

        public MessagesResponse Map(List<SummaryMessage> source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();

            var senderMessages = source.OrderByDescending(s => s.SentTime)
                .Select(s => new SenderMessages
                {
                    Sender = s.Sender,
                    UnreadCount = s.UnreadCount,
                    Messages = new List<Message>
                    {
                        new Message
                        {
                            Id = s.Id,
                            Sender = s.Sender,
                            Version = s.Version,
                            Body = s.Body,
                            Read = s.ReadTime.HasValue,
                            SentTime = s.SentTime,
                        }
                    }
                });

            return new MessagesResponse(senderMessages);
        }
    }
}