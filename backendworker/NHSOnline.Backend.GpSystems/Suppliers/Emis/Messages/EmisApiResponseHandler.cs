using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    internal sealed class EmisApiResponseHandler<TBody, TResult>
    {
        private readonly ILogger _logger;
        private readonly Func<EmisApiObjectResponse<TBody>, TResult> _success;
        private readonly Func<TResult> _forbidden;
        private readonly Func<TResult> _badRequest;
        private readonly Func<TResult> _unknown;

        internal EmisApiResponseHandler(
            ILogger logger,
            Func<EmisApiObjectResponse<TBody>, TResult> success,
            Func<TResult> forbidden,
            Func<TResult> badRequest,
            Func<TResult> unknown)
        {
            _logger = logger;
            _success = success;
            _forbidden = forbidden;
            _badRequest = badRequest;
            _unknown = unknown;
        }

        internal TResult Handle(EmisApiObjectResponse<TBody> response)
        {
            if (response.HasSuccessResponse)
            {
                return _success(response);
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(response);
                return _forbidden();
            }

            if (response.HasBadRequestResponse)
            {
                _logger.LogEmisErrorResponse(response);
                return _badRequest();
            }

            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);
            return _unknown();
        }
    }
}