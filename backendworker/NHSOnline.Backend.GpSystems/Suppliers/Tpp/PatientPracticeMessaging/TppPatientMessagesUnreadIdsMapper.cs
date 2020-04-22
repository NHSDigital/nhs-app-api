using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging
{
    public interface ITppPatientMessagesUnreadIdsMapper
    {
        List<string> Map(List<MessageDetails> messages, string conversationId);
    }

    public class TppPatientMessagesUnreadIdsMapper: ITppPatientMessagesUnreadIdsMapper
    {
        public List<string> Map(List<MessageDetails> messages, string conversationId)
        {
            if (string.IsNullOrWhiteSpace(conversationId))
            {
                throw new ArgumentException($"Argument {nameof(conversationId)} must not be empty");
            }

            return messages
                ?.Where(m =>
                    !string.IsNullOrWhiteSpace(m?.MessageId)
                    && m.ConversationId == conversationId
                    && m.Read == YesNo.n)
                .Select(m => m.MessageId)
                .ToList() ?? new List<string>();
        }
    }
}