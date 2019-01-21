using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.Support;

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
    }
}
