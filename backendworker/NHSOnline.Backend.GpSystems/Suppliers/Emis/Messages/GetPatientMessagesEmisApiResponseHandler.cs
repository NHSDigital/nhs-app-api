using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    internal sealed class GetPatientMessagesEmisApiResponseHandler
    {
        private readonly ILogger _logger;
        private readonly IEmisPatientMessagesMapper _messageListMapper;
        private readonly EmisApiResponseHandler<MessagesGetResponse, GetPatientMessagesResult> _handler;

        public GetPatientMessagesEmisApiResponseHandler(ILogger logger, IEmisPatientMessagesMapper messageListMapper)
        {
            _logger = logger;
            _messageListMapper = messageListMapper;
            _handler = new EmisApiResponseHandler<MessagesGetResponse, GetPatientMessagesResult>(
                logger,
                Success,
                Forbidden,
                BadRequest,
                Unknown);
        }

        public GetPatientMessagesResult Handle(EmisApiObjectResponse<MessagesGetResponse> response)
            => _handler.Handle(response);

        private GetPatientMessagesResult Success(
            EmisApiObjectResponse<MessagesGetResponse> response)
        {
            _logger.LogInformation("Mapping EMIS patient message summaries");
            var mapped = _messageListMapper.Map(response.Body);

            if (mapped == null)
            {
                _logger.LogInformation("Mapping EMIS patient message summaries returned null");
                return new GetPatientMessagesResult.BadGateway();
            }

            _logger.LogInformation($"Number of EMIS patient message summaries retrieved: {mapped.MessageSummaries.Count}");

            return new GetPatientMessagesResult.Success(mapped);
        }

        private static GetPatientMessagesResult Forbidden() => new GetPatientMessagesResult.Forbidden();

        private static GetPatientMessagesResult BadRequest() => new GetPatientMessagesResult.BadRequest();

        private static GetPatientMessagesResult Unknown() => new GetPatientMessagesResult.BadGateway();
    }
}