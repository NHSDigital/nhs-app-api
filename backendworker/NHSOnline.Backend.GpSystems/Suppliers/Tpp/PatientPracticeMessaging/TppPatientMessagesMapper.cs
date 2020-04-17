using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.Support.Logging;

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
            if (response == null)
            {
                _logger.LogError("MessagesViewReply is null, throwing null argument exception");
                _logger.LogExit();
                throw new ArgumentNullException(nameof(response));
            }

            if (response.Messages == null || response.Messages.Count == 0)
            {
                _logger.LogError("Messages is empty or null, returning");
                _logger.LogExit();
                return new GetPatientMessagesResponse
                {
                    MessageSummaries = new List<PatientMessageSummary>()
                };
            }

            _logger.LogInformation("Mapping TPP messages");
            _logger.LogExit();

            var summaries = response.Messages
                .Where(m => m.Deleted != YesNo.y)
                .GroupBy(m => m.ConversationId)
                .Select(c =>
                {
                    var messages = c.ToList().OrderBy(m => DateTime.Parse(m.Sent, CultureInfo.InvariantCulture));
                    var initialMessage = c.First();
                    var unreadCount = GetUnreadCount(messages, c.Key);

                    return new PatientMessageSummary
                    {
                        MessageId = initialMessage.MessageId,
                        ConversationId = initialMessage.ConversationId,
                        Content = initialMessage.MessageText,
                        Recipient = initialMessage.Incoming != YesNo.y
                            ? initialMessage.Sender
                            : initialMessage.Recipient,
                        Sender = initialMessage.Sender,
                        HasUnreadReplies = unreadCount > 0,
                        UnreadCount = unreadCount,
                        LastMessageDateTime = GetFormattedDateString(messages.Last().Sent),
                        SentDateTime = GetFormattedDateString(initialMessage.Sent),
                        AttachmentId = initialMessage.BinaryDataId,
                        OutboundMessage = initialMessage.Incoming == YesNo.y,
                        Replies = GetReplies (response.Messages, initialMessage.ConversationId)
                    };
                })
                .OrderByDescending(c => c.LastMessageDateTime)
                .ToList();

            return new GetPatientMessagesResponse
            {
                MessageSummaries = summaries
            };
        }

        private static string GetFormattedDateString(String date)
        {
            return DateTime.Parse(
                date,
                CultureInfo.InvariantCulture).ToString(
                "s",
                CultureInfo.InvariantCulture);
        }

        private static int GetUnreadCount(IEnumerable<Message> messages, string conversationId)
        {
            var unreadMessages = messages
                .Where(r => r.ConversationId == conversationId && r.Read != YesNo.y)
                .ToList();
            return unreadMessages.Count;
        }

        private static List<MessageReply> GetReplies(List<Message> messages, string conversationId)
        {
            return messages
                .Where(r => r.MessageId != conversationId && r.ConversationId == conversationId )
                .Select(
                reply => new MessageReply
                {
                    Sender = reply.Sender,
                    SentDateTime = GetFormattedDateString(reply.Sent),
                    IsUnread = reply.Read != YesNo.y,
                    ReplyContent = reply.MessageText,
                    AttachmentId = reply.BinaryDataId,
                    OutboundMessage = reply.Incoming == YesNo.y
                }).ToList();
        }
    }
}