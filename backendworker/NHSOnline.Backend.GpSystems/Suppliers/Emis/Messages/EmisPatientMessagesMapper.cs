using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public class EmisPatientMessagesMapper : IEmisPatientMessagesMapper
    {
        private readonly ILogger<EmisPatientMessagesMapper> _logger;
        
        public EmisPatientMessagesMapper(ILogger<EmisPatientMessagesMapper> logger)
        {
            _logger = logger;
        }
        
        public GetPatientMessagesResponse Map(MessagesGetResponse response)
        {
            try
            {
                return new GetPatientMessagesResponse
                {
                    MessageSummaries = response.Messages
                        .Select(m => new PatientMessageSummary
                        {
                            Id = m.MessageId,
                            Subject = m.Subject,
                            LastMessageDateTime = m.LastReplyDateTime ?? m.SentDateTime,
                            Recipient = GetRecipientFromMessageMetaData(m),
                            ReplyCount = m.ReplyCount,
                            HasUnreadReplies = m.HasUnreadReplies
                        })
                        .OrderByDescending(pm => pm.LastMessageDateTime)
                        .ToList()
                };
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, "Failed to map patient message summaries response from EMIS");
                return null;
            }
        }

        private string GetRecipientFromMessageMetaData(MessageSummary messageSummary)
        {
            _logger.LogEnter();

            try
            {
                var currentOwner = messageSummary.Recipients?.FirstOrDefault()?.Name;
                if (!string.IsNullOrWhiteSpace(currentOwner))
                {
                    return currentOwner;
                }
                
                _logger.LogError(
                    $"Unable to retrieve name from first recipient for message summary with id: {messageSummary.MessageId}");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unknown error occured when getting message recipient name(s)");
                return null;
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}