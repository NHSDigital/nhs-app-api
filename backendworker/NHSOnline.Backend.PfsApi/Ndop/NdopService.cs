using System;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Ndop.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.PfsApi.Ndop
{
    public class NdopService: INdopService
    {
        private readonly ILogger<NdopService> _logger;
        private readonly ISigning _signing;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IConfiguration _configuration;

        private const string ClaimTypeNhsNumber = "nhs_number";

        public NdopService(ISigning signing, IJwtTokenGenerator jwtTokenGenerator, IConfiguration configuration, ILogger<NdopService> logger)
        {
            _signing = signing;
            _jwtTokenGenerator = jwtTokenGenerator;
            _configuration = configuration;
            _logger = logger;
        }

        public GetNdopResult GetJwtToken(string nhsNumber)
        {
            try
            {
                var signingCredentials = _signing.GetSigningCredentials("NDOP");
                var claimAudience = _configuration.GetOrWarn("NDOP_CLAIM_AUDIENCE", _logger);
                var claimIssuer = _configuration.GetOrWarn("NDOP_CLAIM_ISSUER", _logger);

                var isValid = new ValidateAndLog(_logger)
                    .IsNotNull(signingCredentials, nameof(signingCredentials))
                    .IsNotNullOrWhitespace(claimAudience, nameof(claimAudience))
                    .IsNotNullOrWhitespace(claimIssuer, nameof(claimIssuer))
                    .IsValid();

                if (!isValid)
                {
                    _logger.LogError("Could not get Ndop claim audience/issuer.");
                    return new GetNdopResult.InternalServerError();
                }

                var claims = new[]
                {
                    new Claim(ClaimTypeNhsNumber, nhsNumber.RemoveWhiteSpace())
                };

                var expiryTime = DateTime.UtcNow.AddSeconds(30);

                var token = _jwtTokenGenerator.GenerateJwtSecurityToken(
                    signingCredentials, expiryTime, claimAudience, claimIssuer, claims);

                return new GetNdopResult.Success(new NdopResponse { Token = token });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new GetNdopResult.InternalServerError();
            }
        }
    }
}