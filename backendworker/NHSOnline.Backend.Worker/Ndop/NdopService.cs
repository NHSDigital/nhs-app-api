using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Ndop.Models;

namespace NHSOnline.Backend.Worker.Ndop
{
    public class NdopService: INdopService
    {
        private readonly ILogger<NdopService> _logger;
        private readonly INdopSigning _ndopSigning;
        private readonly IConfiguration _configuration;

        private const string ClaimTypeNhsNumber = "nhs_number";
        
        public NdopService(INdopSigning ndopSigning, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _ndopSigning = ndopSigning;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<NdopService>();
        }
        
        public async Task<GetNdopResult> GetJwtToken(string nhsNumber)
        {
            return await Task.Run(() => GetToken(nhsNumber));
        }

        public GetNdopResult GetToken(string nhsNumber)
        {           
            try
            {
                var signingCredentials = _ndopSigning.GetSigningCredentials();
                if (signingCredentials == null)
                    return new GetNdopResult.Unsuccessful();
                
                var claims = new[]
                {
                    new Claim(ClaimTypeNhsNumber, nhsNumber)
                };

                var expiryTime = DateTime.UtcNow.AddSeconds(30);
                
                var claimAudience = _configuration.GetOrWarn("NDOP_CLAIM_AUDIENCE", _logger);
                var claimIssuer = _configuration.GetOrWarn("NDOP_CLAIM_ISSUER", _logger);

                if (string.IsNullOrEmpty(claimAudience) || string.IsNullOrEmpty(claimIssuer))
                {
                    _logger.LogError("Could not get Ndop claim audience/issuer.");
                    return new GetNdopResult.Unsuccessful();
                }

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