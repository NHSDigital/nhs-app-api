using System;
using System.Net;
using System.Text.RegularExpressions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public abstract class TppApiResponse : ApiResponse
    {
        private static readonly Regex ErrorRegex = new Regex("errorCode\\s?=");

        protected TppApiResponse(HttpStatusCode status) :base(status)
        {}

        public Error ErrorResponse { get; set; }

        public override bool HasSuccessResponse => ErrorResponse == null && StatusCode.IsSuccessStatusCode();

        internal bool IsUnauthorisedResponse => HasErrorWithCode(TppApiErrorCodes.NotAuthenticated);

        public bool HasForbiddenResponse => HasErrorWithCode(TppApiErrorCodes.NoAccess);

        public bool HasErrorWithCode(string errorCode)
        {
            return string.Equals(ErrorResponse?.ErrorCode, errorCode, StringComparison.Ordinal);
        }

        public bool HasErrorMessageContaining(string message)
        {
            return ErrorResponse?.UserFriendlyMessage.Contains(message, StringComparison.Ordinal) ?? false;
        }

        protected static bool IsErrorResponse(string responseString)
        {
            return ErrorRegex.IsMatch(responseString);
        }
    }
}