using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public static class TppErrorMapper
    {
        public static TEnum? Map<TResponse, TEnum>(ILogger logger, TppClient.TppApiObjectResponse<TResponse> response,
            Dictionary<string, TEnum> keyToEnumMapper) where TEnum : struct
        {
            logger.LogTppErrorResponse(response);
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var statusCode = (int?)response.StatusCode;
            var tppErrorCode = response.ErrorResponse?.ErrorCode;
            var key = $"{statusCode}{tppErrorCode}";

            var successfulKeyMapping = keyToEnumMapper.TryGetValue(key, out var keyMapping);
            logger.LogInformation(successfulKeyMapping
                ? $"Mapping found with Key '{key}'"
                : $"No mapping found with Key '{key}'");
            return successfulKeyMapping ? keyMapping : (TEnum?)null;
        }
    }
}