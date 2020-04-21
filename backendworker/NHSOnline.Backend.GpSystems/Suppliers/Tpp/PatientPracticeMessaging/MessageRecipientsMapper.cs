using System;
using System.Linq;
using Microsoft.Azure.Documents.SystemFunctions;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging
{
    public interface IMessageRecipientsMapper
    {
        PatientPracticeMessageRecipients Map(MessageRecipientsReply messageRecipientsReply);
    }

    public class MessageRecipientsMapper : IMessageRecipientsMapper
    {

        public PatientPracticeMessageRecipients Map(MessageRecipientsReply messageRecipientsReply)
        {
            if (messageRecipientsReply is null)
            {
                throw new ArgumentNullException(nameof(messageRecipientsReply));
            }

            var recipientList = messageRecipientsReply.Items.Select(
                item => new MessageRecipient {
                    Name = item.ItemText,
                    RecipientIdentifier = $"{item.Id}:{item.Description}"
                }).ToList();

            return new PatientPracticeMessageRecipients
            {
                MessageRecipients = recipientList,
                HasErrored = false
            };
        }
    }
}