using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    internal sealed class PostPatientMessageEmisApiResponseHandler
    {
        private readonly IEmisPatientMessageSendMapper _messageSendWrapper;
        private readonly EmisApiResponseHandler<MessagePostResponse, PostPatientMessageResult> _handler;

        public PostPatientMessageEmisApiResponseHandler(ILogger logger, IEmisPatientMessageSendMapper messageSendWrapper)
        {
            _messageSendWrapper = messageSendWrapper;
            _handler = new EmisApiResponseHandler<MessagePostResponse, PostPatientMessageResult>(
                logger,
                Success,
                Forbidden,
                BadRequest,
                Unknown);
        }

        public PostPatientMessageResult Handle(EmisApiObjectResponse<MessagePostResponse> response)
            => _handler.Handle(response);

        private PostPatientMessageResult Success(
            EmisApiObjectResponse<MessagePostResponse> response)
        {
            var mapped = _messageSendWrapper.Map(response.Body);
            if (mapped.HasValue)
            {
                return new PostPatientMessageResult.Success();
            }

            return new PostPatientMessageResult.BadRequest();
        }

        private static PostPatientMessageResult Forbidden() => new PostPatientMessageResult.Forbidden();

        private static PostPatientMessageResult BadRequest() => new PostPatientMessageResult.BadRequest();

        private static PostPatientMessageResult Unknown() => new PostPatientMessageResult.BadGateway();
    }
}