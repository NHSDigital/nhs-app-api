using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Ndop.Models;

namespace NHSOnline.Backend.Worker.Ndop
{
    public class NdopService: INdopService
    {
        private readonly ILogger<NdopService> _logger;
        private readonly INdopSigning _ndopSigning;
        
        public NdopService(INdopSigning ndopSigning, ILoggerFactory loggerFactory)
        {
            _ndopSigning = ndopSigning;
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
                    new Claim(ClaimTypes.NameIdentifier, nhsNumber)
                };

                var jwtToken = new JwtSecurityToken(
                    issuer: "testdomain.com",
                    audience: "testdomain.com",
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(30),
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