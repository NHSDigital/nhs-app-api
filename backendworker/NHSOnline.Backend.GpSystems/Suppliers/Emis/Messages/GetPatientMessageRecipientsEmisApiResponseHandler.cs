using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    internal sealed class GetPatientMessageRecipientsEmisApiResponseHandler
    {
        private readonly ILogger _logger;
        private readonly IEmisPatientMessageRecipientsMapper _messageRecipientsMapper;
        private readonly EmisApiResponseHandler<MessageRecipientsResponse, GetPatientMessageRecipientsResult> _handler;

        public GetPatientMessageRecipientsEmisApiResponseHandler(ILogger logger, IEmisPatientMessageRecipientsMapper messageRecipientsMapper)
        {
            _logger = logger;
            _messageRecipientsMapper = messageRecipientsMapper;
            _handler = new EmisApiResponseHandler<MessageRecipientsResponse, GetPatientMessageRecipientsResult>(
                logger,
                Success,
                Forbidden,
                BadRequest,
                Unknown);
        }

        public GetPatientMessageRecipientsResult Handle(EmisApiObjectResponse<MessageRecipientsResponse> response)
            => _handler.Handle(response);

        private GetPatientMessageRecipientsResult Success(
            EmisApiObjectResponse<MessageRecipientsResponse> response)
        {
            var messageRecipientsResponse = response.Body;

            _logger.LogInformation($"Number of recipients: {messageRecipientsResponse?.MessageRecipients?.Count}");
            return new GetPatientMessageRecipientsResult.Success(_messageRecipientsMapper.Map(messageRecipientsResponse));
        }

        private static GetPatientMessageRecipientsResult Forbidden() => new GetPatientMessageRecipientsResult.Forbidden();

        private static GetPatientMessageRecipientsResult BadRequest() => new GetPatientMessageRecipientsResult.BadRequest();

        private static GetPatientMessageRecipientsResult Unknown() => new GetPatientMessageRecipientsResult.BadGateway();
    }
}