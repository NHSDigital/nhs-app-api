using System;
using Microsoft.Azure.Documents.SystemFunctions;
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
            
            var visionErrorMessage = response.ErrorMessage;
            var statusCode = (int)response.StatusCode;

            if (response.IsInvalidRequestError)
            {
                return keyAndMessageTo.Map(logger, 
                    $"{statusCode}{VisionApiFaultCodes.InvalidRequest}", 
                    "Vision Im1Connection error of type 'Invalid Request'.");
            }
            
            if (response.IsInvalidSecurityHeaderError)
            {
                return keyAndMessageTo.Map(logger, 
                    $"{statusCode}{VisionApiFaultCodes.InvalidSecurity}", 
                    "Vision Im1Connection error of type 'Invalid Security Error'.");
            }
            
            var visionErrorCode = response.ErrorCode;
            var key = $"{statusCode}{visionErrorCode}";
            return keyAndMessageTo.Map(logger, key, visionErrorMessage);
        }
    }
}