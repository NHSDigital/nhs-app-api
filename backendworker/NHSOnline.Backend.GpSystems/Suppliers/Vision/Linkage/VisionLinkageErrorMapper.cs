using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage
{
    public abstract class VisionLinkageErrorMapper<T> : LinkageErrorMapper
    {
        protected abstract KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.Code> KeyAndMessageToError { get; }

        public LinkageResult Map(VisionLinkageClient.VisionApiObjectResponse<T> response, ILogger<VisionLinkageService> logger)
        {
            logger.LogVisionErrorResponse(response);
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var statusCode = (int)response.StatusCode;
            var visionErrorCode = response.ErrorResponse?.Code;
            var visionErrorMessage = response.ErrorResponse?.Text;
            var key = $"{statusCode}{visionErrorCode}";

            var mappedValue = KeyAndMessageToError.Map(logger, key, visionErrorMessage);

            return mappedValue != null ? new LinkageResult.ErrorCase(mappedValue.Value) : MapUnknownError(response.StatusCode);
        }
    }
}