using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.Support.Logging;
using MessageDetails = NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging.MessageDetails;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging
{

    public interface ITppPatientMessagesMapper
    {
        GetPatientMessagesResponse Map(MessagesViewReply response);
    }

    public class TppPatientMessagesMapper: ITppPatientMessagesMapper
    {
        private readonly ILogger<TppPatientMessagesMapper> _logger;

        public TppPatientMessagesMapper(ILogger<TppPatientMessagesMapper> logger)
        {
            _logger = logger;
        }

        public GetPatientMessagesResponse Map(MessagesViewReply response)
        {
            _logger.LogEnter();

            if (response == null)
            {
                _logger.LogError("MessagesViewReply is null, throwing null argument exception");
                _logger.LogExit();

                throw new ArgumentNullException(nameof(response));
            }

            if ((response.Messages?.Count ?? 0) == 0)
            {
                _logger.LogError("Messages is empty or null, returning");
                _logger.LogExit();

                return new GetPatientMessagesResponse
                {
                    MessageSummaries = new List<PatientMessageSummary>()
                };
            }

            _logger.LogInformation("Mapping TPP messages");

            var summaries = response.Messages
                .Where(m => m.Deleted != YesNo.y && m.HasSentDateTime)
                .GroupBy(m => m.ConversationId)
                .Select(c =>
                {
                    var orderedMessages = c.ToList()
                        .OrderBy(m => m.SentDateTime)
                        .ToList();
                    var unreadMessages = orderedMessages.Where(r =>
                            r.Incoming == YesNo.n && r.Read == YesNo.n)
                        .ToList();
                    var initialMessage = orderedMessages.First();
                    var unreadCount = unreadMessages.Count;
                    var lastMessageSentDateTime = orderedMessages.Last().SentDateTime;

                    return (
                        buildMessageSummary(initialMessage, unreadCount, lastMessageSentDateTime, orderedMessages),
                        lastMessageSentDateTime
                    );
                })
                .OrderByDescending(m => m.Item2)
                .Select(m => m.Item1)
                .ToList();

            _logger.LogExit();

            return new GetPatientMessagesResponse
            {
                MessageSummaries = summaries
            };
        }

        private PatientMessageSummary buildMessageSummary(
            MessageDetails initialMessageDetails,
            int unreadCount,
            DateTime lastMessageSentDateTime,
            List<MessageDetails> orderedMessages
        )
        {
            return new PatientMessageSummary
            {
                MessageId = initialMessageDetails.ConversationId,
                Content = initialMessageDetails.MessageText,
                Recipient = initialMessageDetails.Incoming == YesNo.n
                    ? initialMessageDetails.Sender
                    : initialMessageDetails.Recipient,
                Sender = initialMessageDetails.Sender,
                IsUnread = initialMessageDetails.Incoming == YesNo.n
                           && initialMessageDetails.Read == YesNo.n,
                UnreadReplyInfo = new UnreadReplyInfo
                {
                    Present = unreadCount > 0,
                    Count = unreadCount,
                },
                LastMessageDateTime = GetFormattedDateString(lastMessageSentDateTime),
                SentDateTime = GetFormattedDateString(initialMessageDetails.SentDateTime),
                AttachmentId = initialMessageDetails.BinaryDataId,
                OutboundMessage = initialMessageDetails.Incoming == YesNo.y,
                Replies = GetReplies(orderedMessages, initialMessageDetails.ConversationId)
            };
        }

        private static List<MessageReply> GetReplies(List<MessageDetails> messages, string conversationId)
        {
            return messages
                .Where(r => r.MessageId != conversationId )
                .Select(reply => new MessageReply
                    {
                        Sender = reply.Sender,
                        SentDateTime = GetFormattedDateString(reply.SentDateTime),
                        IsUnread = reply.Read != YesNo.y,
                        ReplyContent = reply.MessageText,
                        AttachmentId = reply.BinaryDataId,
                        OutboundMessage = reply.Incoming == YesNo.y
                    }).ToList();
        }

        private static string GetFormattedDateString(DateTime date)
        {
            return date.ToString(
                "s",
                CultureInfo.InvariantCulture);
        }
    }
}