using System;
using System.Linq;
using System.Net;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public abstract class EmisApiResponse : ApiResponse
    {
        protected EmisApiResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public string RawResponse { get; protected internal set; }
        public StandardErrorResponse StandardErrorResponse { get; set; }
        public ExceptionErrorResponse ExceptionErrorResponse { get; set; }
        public BadRequestErrorResponse ErrorResponseBadRequest { get; set; }
        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

        public bool HasBadRequestResponse => StatusCode.IsBadRequestCode();

        public bool HasInternalErrorCode(EmisApiErrorCode code)
        {
            return StandardErrorResponse?.InternalResponseCode == (int)code;
        }

        public bool HasStatusCodeAndErrorCode(HttpStatusCode statusCode, EmisApiErrorCode emisApiErrorCode)
        {
            return (StatusCode == statusCode) && HasInternalErrorCode(emisApiErrorCode);
        }

        public bool HasExceptionWithMessage(string message)
        {
            return ExceptionErrorResponse?.Exceptions?.Any(x =>
                string.Equals(x.Message, message, StringComparison.Ordinal)) ?? false;
        }


        public bool HasExceptionWithMessageContaining(string message)
        {
            return ExceptionErrorResponse?.Exceptions?.Any(x =>
                x.Message.Contains(message, StringComparison.Ordinal)) ?? false;
        }

        public bool HasForbiddenResponse()
        {
            if (StatusCode == HttpStatusCode.Forbidden)
            {
                return true;
            }

            return HasExceptionWithMessageContaining(
                EmisApiErrorMessages.EmisService_NotEnabledForUser);
        }

        public string GetExceptionLogMessage(string methodCall)
        {
            var baseMessage = $"Emis {methodCall} returned with status code {StatusCode}";

            if (string.IsNullOrEmpty(ExceptionErrorResponse?.Exceptions?.FirstOrDefault()?.Message))
            {
                return baseMessage + " and no error message";
            }

            return baseMessage
                   + " and error message "
                   + $"{ExceptionErrorResponse?.Exceptions.First().Message}";
        }

        public string ErrorForLogging => $"Error Code: '{StatusCode}'. " +
                                         $"Error Message:'{StandardErrorResponse?.Message}'. ";

    }
}