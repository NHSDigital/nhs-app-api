using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Ndop.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.Ndop
{
    public class NdopService: INdopService
    {
        private readonly ILogger<NdopService> _logger;
        private readonly INdopSigning _ndopSigning;
        private readonly IConfiguration _configuration;

        private const string ClaimTypeNhsNumber = "nhs_number";
        
        public NdopService(INdopSigning ndopSigning, IConfiguration configuration, ILogger<NdopService> logger)
        {
            _ndopSigning = ndopSigning;
            _configuration = configuration;
            _logger = logger;
        }

        public GetNdopResult GetJwtToken(string nhsNumber)
        {
            try
            {
                var signingCredentials = _ndopSigning.GetSigningCredentials();
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
                    return new GetNdopResult.Unsuccessful();
                }
                
                var claims = new[]
                {
                    new Claim(ClaimTypeNhsNumber, nhsNumber.RemoveWhiteSpace())
                };

                var expiryTime = DateTime.UtcNow.AddSeconds(30);

                

                var jwtToken = new JwtSecurityToken(
                    audience: claimAudience,
                    issuer: claimIssuer,
                    claims: claims,
                    expires: expiryTime,
                    signingCredentials: signingCredentials
                );

                var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                return new GetNdopResult.SuccessfullyRetrieved(new NdopResponse { Token = token });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new GetNdopResult.Unsuccessful();
            }
        }
    }
}