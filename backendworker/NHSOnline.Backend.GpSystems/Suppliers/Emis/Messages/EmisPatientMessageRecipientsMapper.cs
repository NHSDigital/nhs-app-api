using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public class EmisPatientMessageRecipientsMapper : IEmisPatientMessageRecipientsMapper
    {
        private readonly ILogger<IEmisPatientMessageRecipientsMapper> _logger;

        public EmisPatientMessageRecipientsMapper(ILogger<IEmisPatientMessageRecipientsMapper> logger)
        {
            _logger = logger;
        }

        public MessageRecipientsResponse Map(MessageRecipientsResponse response)
        {
            var existingIds = new HashSet<string>();

            var mappedResponse = new MessageRecipientsResponse
            {
                MessageRecipients = response?.MessageRecipients?
                    .Where(r =>
                    {
                        var recipientGuid = r?.RecipientIdentifier;

                        if (existingIds.Contains(recipientGuid))
                        {
                            _logger.LogInformation($"Duplicate recipient id {recipientGuid} removed from response");
                            return false;
                        }

                        existingIds.Add(recipientGuid);
                        return true;
                    })
                    .ToList()
            };

            _logger.LogInformation($"Number of mapped recipients: {existingIds.Count}");

            return mappedResponse;
        }
    }
}