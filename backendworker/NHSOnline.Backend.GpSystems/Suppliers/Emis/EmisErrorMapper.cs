using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public static class EmisErrorMapper
    {
        public static TEnum? Map<TResponse, TEnum>(ILogger logger, EmisApiObjectResponse<TResponse> response,
            KeyAndMessageToEnumMapper<TEnum> keyAndMessageTo) where TEnum : struct
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            logger.LogEmisErrorResponse(response);
            var statusCode = (int) response.StatusCode;

            var errorMessages = ErrorMessages(response).ToArray();
            logger.LogInformation($"Found the following messages: {string.Join(",", errorMessages)}");

            return Map(response.StandardErrorResponse?.InternalResponseCode,
                       statusCode, logger, errorMessages, keyAndMessageTo) ??
                   Map(response.ExceptionErrorResponse?.InternalResponseCode,
                       statusCode, logger, errorMessages, keyAndMessageTo) ??
                   keyAndMessageTo.Map(logger, $"{statusCode}", errorMessages);
        }

        public static TEnum? Map<TEnum>(int? emisInternalErrorCode, int statusCode,
            ILogger logger,
            string[] errorMessages,
            KeyAndMessageToEnumMapper<TEnum> keyAndMessageTo) where TEnum : struct
        {
            if (emisInternalErrorCode != null)
            {
                var emisErrorCode = ResolveErrorCode(emisInternalErrorCode);
                var key = $"{statusCode}{emisErrorCode}";
                return keyAndMessageTo.Map(logger, key, errorMessages);
            }
            return null;
        }

        private static string ResolveErrorCode(int? errorCode)
        {
            return errorCode != null ? $"{Math.Abs(errorCode.Value)}" : string.Empty;
        }

        public static IEnumerable<string> ErrorMessages<TResponse>(
            EmisApiObjectResponse<TResponse> response)
        {
            var messages = new[]
            {
                response.StandardErrorResponse?.Message,
                response.ExceptionErrorResponse?.Exceptions?.FirstOrDefault()?.Message
            };
            return messages.Where(message => !string.IsNullOrWhiteSpace(message));
        }
    }
}
