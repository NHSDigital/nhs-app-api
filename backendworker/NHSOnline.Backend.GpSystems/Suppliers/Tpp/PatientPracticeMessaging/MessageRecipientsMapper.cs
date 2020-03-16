using System;
using System.Linq;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging
{
    public interface IMessageRecipientsMapper
    {
        MessageRecipientsResponse Map(MessageRecipientsReply messageRecipientsReply);
    }

    public class MessageRecipientsMapper : IMessageRecipientsMapper
    {

        public MessageRecipientsResponse Map(MessageRecipientsReply messageRecipientsReply)
        {
            if (messageRecipientsReply == null)
            {
                throw new ArgumentNullException(nameof(messageRecipientsReply));
            }

            var recipientList = messageRecipientsReply.Items.Select(
                item => new MessageRecipient { Name = item.ItemText, RecipientIdentifier = item.Id, }).ToList();

            return new MessageRecipientsResponse
            {
                MessageRecipients = recipientList,
                HasErrored = false
            };
        }
    }
}