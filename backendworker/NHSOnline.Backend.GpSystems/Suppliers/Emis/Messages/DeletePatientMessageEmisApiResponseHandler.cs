using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    internal sealed class DeletePatientMessageEmisApiResponseHandler
    {
        private readonly EmisApiResponseHandler<MessageDeleteResponse, DeletePatientMessageResult> _handler;

        public DeletePatientMessageEmisApiResponseHandler(ILogger logger)
        {
            _handler = new EmisApiResponseHandler<MessageDeleteResponse, DeletePatientMessageResult>(
                logger,
                Success,
                Forbidden,
                BadRequest,
                Unknown);
        }

        public DeletePatientMessageResult Handle(EmisApiObjectResponse<MessageDeleteResponse> response)
            => _handler.Handle(response);

        private static DeletePatientMessageResult Success(EmisApiObjectResponse<MessageDeleteResponse> response)
        {
            if (response.Body?.IsDeleted != null && !response.Body.IsDeleted.Value)
            {
                return new DeletePatientMessageResult.BadGateway();
            }

            return new DeletePatientMessageResult.Success();
        }

        private static DeletePatientMessageResult Forbidden() => new DeletePatientMessageResult.Forbidden();

        private static DeletePatientMessageResult BadRequest() => new DeletePatientMessageResult.BadRequest();

        private static DeletePatientMessageResult Unknown() => new DeletePatientMessageResult.BadGateway();
    }
}