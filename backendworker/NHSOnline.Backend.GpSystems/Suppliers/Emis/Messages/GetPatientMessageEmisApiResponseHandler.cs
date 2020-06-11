using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    internal sealed class GetPatientMessageEmisApiResponseHandler
    {
        private readonly ILogger _logger;
        private readonly IEmisPatientMessageMapper _messageMapper;
        private readonly EmisApiResponseHandler<MessageGetResponse, GetPatientMessageResult> _handler;

        public GetPatientMessageEmisApiResponseHandler(ILogger logger, IEmisPatientMessageMapper messageMapper)
        {
            _logger = logger;
            _messageMapper = messageMapper;
            _handler = new EmisApiResponseHandler<MessageGetResponse, GetPatientMessageResult>(
                logger,
                Success,
                Forbidden,
                BadRequest,
                Unknown);
        }

        public GetPatientMessageResult Handle(EmisApiObjectResponse<MessageGetResponse> response)
            => _handler.Handle(response);

        private GetPatientMessageResult Success(
            EmisApiObjectResponse<MessageGetResponse> response)
        {
            _logger.LogInformation("Mapping EMIS patient message details");
            var mapped = _messageMapper.Map(response.Body);

            if (mapped == null)
            {
                _logger.LogInformation("Mapping EMIS patient message details returned null");
                return new GetPatientMessageResult.BadGateway();
            }

            if (mapped.MessageDetails.Replies != null)
            {
                _logger.LogInformation($"Number of replies for message with id {mapped.MessageDetails.MessageId} is {mapped.MessageDetails.Replies.Count}");
            }
            return new GetPatientMessageResult.Success(mapped);
        }

        private static GetPatientMessageResult Forbidden() => new GetPatientMessageResult.Forbidden();

        private static GetPatientMessageResult BadRequest() => new GetPatientMessageResult.BadRequest();

        private static GetPatientMessageResult Unknown() => new GetPatientMessageResult.BadGateway();
    }
}