using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public class EmisPatientMessageMapper : IEmisPatientMessageMapper
    {
        private readonly ILogger<EmisPatientMessageMapper> _logger;
        
        public EmisPatientMessageMapper(ILogger<EmisPatientMessageMapper> logger)
        {
            _logger = logger;
        }
        
        public GetPatientMessageResponse Map(MessageGetResponse response)
        {
            try
            {
                return new GetPatientMessageResponse
                {
                    MessageDetails = {
                        MessageId = response.Message.MessageId,
                        Subject = response.Message.Subject,
                        Recipient = GetRecipientFromMessageMetaData(response),
                        MessageReplies = response.Message.MessageReplies,
                        Content = response.Message.Content,
                        SentDateTime = response.Message.SentDateTime,
                    }
                };
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, "Failed to map patient message details response from EMIS");
                return null;
            }
        }
        
        private string GetRecipientFromMessageMetaData(MessageGetResponse messageResponse)
        {
            _logger.LogEnter();

            try
            {
                var currentOwner = messageResponse.Message.Recipients?.FirstOrDefault()?.Name;
                if (!string.IsNullOrWhiteSpace(currentOwner))
                {
                    return currentOwner;
                }
                
                _logger.LogError(
                    $"Unable to retrieve name from first recipient for message response with id: " +
                    $"{messageResponse.Message.MessageId}");
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