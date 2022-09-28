using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.Messages.Areas.Messages.Mappers
{
    public class MessagesResponseMapper :
        IMapper<List<UserMessage>, MessagesResponse>,
        IMapper<UserMessage, MessagesResponse>,
        IMapper<List<SummaryMessage>, MessagesResponse>
    {
        private const int MaxCharacterLimit = 240;

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
                SenderMessages = new List<SenderMessages>
                {
                    new SenderMessages
                    {
                        Sender = source.First().Sender,
                        UnreadCount = source.Count(m => !m.ReadTime.HasValue),
                        Messages = source.OrderByDescending(m => m.SentTime)
                            .Select(m => MapMessage(m, true))
                            .ToList()
                    }
                }
            };
        }

        public MessagesResponse Map(UserMessage source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();

            return new MessagesResponse
            {
                SenderMessages = new List<SenderMessages>
                {
                    new SenderMessages
                    {
                        Sender = source.Sender,
                        Messages = new List<Message> {
                            MapMessage(source)
                        }
                    }
                }
            };
        }

        public MessagesResponse Map(List<SummaryMessage> source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();

            return new MessagesResponse
            {
                SenderMessages = source.OrderByDescending(s => s.SentTime)
                    .Select(s => new SenderMessages
                    {
                        Sender = s.Sender,
                        UnreadCount = s.UnreadCount,
                        Messages = new List<Message> { MapMessage(s) }
                    })
                    .ToList()
            };
        }

        private Message MapMessage(UserMessage userMessage, bool truncateBody = false)
        {
            return new Message
            {
                Id = userMessage.Id.ToString(),
                SenderId = userMessage.SenderContext?.SenderId,
                Sender = userMessage.Sender,
                Version = userMessage.Version,
                Body = truncateBody
                    ? userMessage.Body.Substring(0, Math.Min(userMessage.Body.Length, MaxCharacterLimit))
                    : userMessage.Body,
                Read = userMessage.ReadTime.HasValue,
                SentTime = userMessage.SentTime,
                Reply = MapMessageReply(userMessage.Reply),
            };
        }

        private MessageReply MapMessageReply(UserMessageReply userMessageReply)
        {
            if (userMessageReply != null)
            {
                return new MessageReply()
                {
                    Options = MapMessageReplyOptions(userMessageReply.Options),
                    Response = userMessageReply.Response,
                    ResponseSentDateTime = userMessageReply.ResponseSentDateTime
                };
            }
            return null;
        }

        private List<ReplyOption> MapMessageReplyOptions(IReadOnlyCollection<UserReplyOption> userReplyOption)
        {
            return userReplyOption?.Select(o => new ReplyOption
            {
                Code = o.Code,
                Display = o.Display
            })
            .ToList();
        }
    }
}
