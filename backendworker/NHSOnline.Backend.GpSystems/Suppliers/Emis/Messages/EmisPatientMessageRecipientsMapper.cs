using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public class EmisPatientMessageRecipientsMapper : IEmisPatientMessageRecipientsMapper
    {
        private readonly ILogger<IEmisPatientMessageRecipientsMapper> _logger;

        public EmisPatientMessageRecipientsMapper(ILogger<IEmisPatientMessageRecipientsMapper> logger)
        {
            _logger = logger;
        }

        public PatientPracticeMessageRecipients Map(MessageRecipientsResponse response)
        {
            var existingIds = new HashSet<string>();

            var recipients = response?.MessageRecipients?
                .Where(r =>
                {
                    var recipientGuid = r?.RecipientGuid;

                    if (existingIds.Contains(recipientGuid))
                    {
                        _logger.LogInformation($"Duplicate recipient id {recipientGuid} removed from response");
                        return false;
                    }

                    existingIds.Add(recipientGuid);
                    return true;
                }).Select(
                    messageRecipient => new MessageRecipient
                    {
                        RecipientIdentifier = messageRecipient.RecipientGuid,
                        Name = messageRecipient.Name
                    }).ToList();

            var mappedResponse = new PatientPracticeMessageRecipients
            {
                MessageRecipients = recipients
            };

            _logger.LogInformation($"Number of mapped recipients: {existingIds.Count}");

            return mappedResponse;
        }
    }
}