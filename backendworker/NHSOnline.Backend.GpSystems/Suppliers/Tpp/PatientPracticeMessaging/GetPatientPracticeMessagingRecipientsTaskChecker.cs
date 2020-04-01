using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging
{
    public interface IGetPatientPracticeMessagingRecipientsTaskChecker
    {
        PatientPracticeMessageRecipients Check(TppApiObjectResponse<MessageRecipientsReply> taskResponse);
    }

    public class GetPatientPracticeMessagingRecipientsTaskChecker : IGetPatientPracticeMessagingRecipientsTaskChecker
    {
        private readonly IMessageRecipientsMapper _messageRecipientsMapper;
        private readonly ILogger<GetPatientPracticeMessagingRecipientsTaskChecker> _logger;

        public GetPatientPracticeMessagingRecipientsTaskChecker(
            IMessageRecipientsMapper messageRecipientsMapper,
            ILogger<GetPatientPracticeMessagingRecipientsTaskChecker> logger)
        {
            _messageRecipientsMapper = messageRecipientsMapper;
            _logger = logger;
        }


        public PatientPracticeMessageRecipients Check(TppApiObjectResponse<MessageRecipientsReply> taskResponse)
        {
            _logger.LogEnter();

            if (taskResponse.HasSuccessResponse)
            {
                _logger.LogDebug("Mapping TPP response to recipients response");
                return _messageRecipientsMapper.Map(taskResponse.Body);
            }

            if (taskResponse.HasForbiddenResponse)
            {
                _logger.LogWarning("User does not have access to their messages for Tpp");
            }
            else
            {
                _logger.LogError(
                    $"Unsuccessful request retrieving the patients message recipients for Tpp. Status code: " +
                    $"{(int)taskResponse.StatusCode}");
            }

            _logger.LogExit();
            return new PatientPracticeMessageRecipients
            {
                HasErrored = true
            };
        }
    }
}