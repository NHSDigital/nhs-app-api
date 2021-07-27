using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public class SessionValidator
    {
        private readonly ILogger _logger;

        public SessionValidator(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsPostValid(UserSessionRequest request)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.AuthCode, nameof(request.AuthCode))
                .IsNotNullOrWhitespace(request.CodeVerifier, nameof(request.CodeVerifier))
                .IsNotNullOrWhitespace(request.RedirectUrl, nameof(request.RedirectUrl))
                .IsValid();
        }

        public bool IsOnDemandGpSessionPostValid(UserSessionRequest request)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.AuthCode, nameof(request.AuthCode))
                .IsNotNullOrWhitespace(request.RedirectUrl, nameof(request.RedirectUrl))
                .IsValid();
        }

        public void LogErrorMessages(UserSessionRequest request)
        {
            var errors = new Dictionary<string, string>();
            AddNonNullString(request.NhsLoginError, nameof(request.NhsLoginError), errors);
            AddNonNullString(request.NhsLoginErrorDescription, nameof(request.NhsLoginErrorDescription), errors);
            AddNonNullString(request.NhsLoginErrorUri, nameof(request.NhsLoginErrorUri), errors);

            if (errors.Count > 0)
            {
                _logger.LogWarningKeyValuePairs("Error parameters received from OnDemandGpSession SSO", errors);
            }
        }

        private static void AddNonNullString(string value, string name, IDictionary<string, string> errors)
        {
            if (!string.IsNullOrEmpty(value))
            {
                errors.Add(name, value);
            }
        }
    }
}
