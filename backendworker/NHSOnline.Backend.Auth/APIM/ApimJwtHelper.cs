using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Auth.APIM
{
    public class ApimJwtHelper : IApimJwtHelper
    {
        private readonly ILogger<ApimJwtHelper> _logger;
        private readonly ICertificateService _certificateService;

        public ApimJwtHelper(
            ILogger<ApimJwtHelper> logger,
            ICertificateService certificateService)
        {
            _logger = logger;
            _certificateService = certificateService;
        }

        public string CreateApimJwt(
            Uri audience,
            string certPath,
            string certPassphrase,
            string key,
            string kid)
        {
            try
            {
                _logger.LogEnter();

                var now = DateTime.UtcNow;

                var certificate = _certificateService.GetCertificate(certPath, certPassphrase);

                var jti = new Claim("jti", Guid.NewGuid().ToString());
                var subject = new Claim(JwtRegisteredClaimNames.Sub, key);

                var token = new JwtSecurityToken(
                    key,
                    audience.AbsoluteUri,
                    new List<Claim>
                    {
                        jti,
                        subject
                    },
                    now,
                    now.AddMinutes(2),
                    new SigningCredentials(
                        new X509SecurityKey(certificate, kid),
                        SecurityAlgorithms.RsaSha512
                    )
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}