using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public static class VisionErrorMapper
    {
        public static TEnum? Map<TResponse, TEnum>(ILogger logger,
            VisionLinkageClient.VisionApiObjectResponse<TResponse> response,
            KeyAndMessageToEnumMapper<TEnum> keyAndMessageTo) where TEnum : struct
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

            return keyAndMessageTo.Map(logger, key, visionErrorMessage);
        }

        public static TEnum? Map<TResponse, TEnum>(ILogger logger,
            VisionPFSClient.VisionApiObjectResponse<TResponse> response,
            KeyAndMessageToEnumMapper<TEnum> keyAndMessageTo) where TEnum : struct
        {
            logger.LogVisionErrorResponse(response);
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var statusCode = (int)response.StatusCode;
            var visionErrorCode = response.ErrorCode;
            var visionErrorMessage = response.ErrorMessage;
            var key = $"{statusCode}{visionErrorCode}";

            return keyAndMessageTo.Map(logger, key, visionErrorMessage);
        }
    }
}