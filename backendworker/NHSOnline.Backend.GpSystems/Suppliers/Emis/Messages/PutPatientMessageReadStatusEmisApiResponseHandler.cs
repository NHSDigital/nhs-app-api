using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    internal sealed class PutPatientMessageReadStatusEmisApiResponseHandler
    {
        private readonly EmisApiResponseHandler<MessageUpdateResponse, PutPatientMessageReadStatusResult> _handler;

        public PutPatientMessageReadStatusEmisApiResponseHandler(ILogger logger)
        {
            _handler = new EmisApiResponseHandler<MessageUpdateResponse, PutPatientMessageReadStatusResult>(
                logger,
                Success,
                Forbidden,
                BadRequest,
                Unknown);
        }

        public PutPatientMessageReadStatusResult Handle(EmisApiObjectResponse<MessageUpdateResponse> response)
            => _handler.Handle(response);

        private static PutPatientMessageReadStatusResult Success(EmisApiObjectResponse<MessageUpdateResponse> _)
            => new PutPatientMessageReadStatusResult.Success();

        private static PutPatientMessageReadStatusResult Forbidden() => new PutPatientMessageReadStatusResult.Forbidden();

        private static PutPatientMessageReadStatusResult BadRequest() => new PutPatientMessageReadStatusResult.BadRequest();

        private static PutPatientMessageReadStatusResult Unknown() => new PutPatientMessageReadStatusResult.BadGateway();
    }
}