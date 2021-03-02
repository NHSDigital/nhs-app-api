using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.PfsApi.Session
{
    public class SessionExpiryCookieCreator: ISessionExpiryCookieCreator
    {
        private readonly ConfigurationSettings _settings;
        private readonly ILogger<ISessionExpiryCookieCreator> _logger;
        private readonly ISigning _signing;
        private readonly AuthSigningConfig _authSigningConfig;
        private readonly ICurrentDateTimeProvider _currentDateTimeProvider;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IWebHostEnvironment _env;

        public SessionExpiryCookieCreator(
            ConfigurationSettings settings,
            ISigning signing,
            AuthSigningConfig authSigningConfig,
            ICurrentDateTimeProvider currentDateTimeProvider,
            IJwtTokenGenerator jwtTokenGenerator,
            IWebHostEnvironment env,
            ILogger<ISessionExpiryCookieCreator> logger)
        {
            _settings = settings;
            _signing = signing;
            _authSigningConfig = authSigningConfig;
            _currentDateTimeProvider = currentDateTimeProvider;
            _jwtTokenGenerator = jwtTokenGenerator;
            _env = env;
            _logger = logger;
        }

        public string CreateSessionExpiryToken()
        {
            var token = GenerateToken();

            var isValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(token, nameof(token))
                .IsValid();

            if (!isValid)
            {
                _logger.LogError("Failed to create session expiry Jwt.");
                return null;
            }

            return token;
        }

        public string DecodeSessionExpiryToken(string encryptedCookie)
        {
            RSAParameters rsaParameters;

            try
            {
                rsaParameters = _signing.GetRsaParameters(_authSigningConfig);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to generate RSA parameters from Auth certificate config");
                return null;
            }

            return _jwtTokenGenerator.DecodeJwtSecurityToken(rsaParameters, encryptedCookie);
        }

        private string GenerateToken()
        {
            RSAParameters rsaParameters;

            try
            {
                rsaParameters = _signing.GetRsaParameters(_authSigningConfig);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to generate RSA parameters from Auth certificate config");
                return null;
            }

            var payload = new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Exp, _currentDateTimeProvider.UtcNow.AddMinutes(_settings.DefaultSessionExpiryMinutes) }
            };

            return _jwtTokenGenerator.GenerateJwtSecurityToken(rsaParameters, payload);
        }

        public void AppendSessionExpiryCookie(HttpContext context, string token)
        {
            var cookieOptions = new CookieOptions()
            {
                Secure = !_env.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                HttpOnly = true
            };

            if (!string.IsNullOrEmpty(_settings.CookieDomain))
            {
                cookieOptions.Domain = _settings.CookieDomain;
            }

            context.Response.Cookies.Append(Constants.CookieNames.SessionExpiry, token, cookieOptions);
        }
    }
}
