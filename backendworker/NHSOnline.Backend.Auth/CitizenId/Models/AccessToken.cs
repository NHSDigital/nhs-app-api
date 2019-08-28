using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.Auth.CitizenId.Models
{
    public class AccessToken
    {
        private const string NhsNumberClaimType = "nhs_number";

        public string Subject { get; }

        public string NhsNumber { get; }

        private AccessToken(ILogger logger, string subject, string nhsNumber)
        {
            new ValidateAndLog(logger)
                .IsNotNullOrWhitespace(subject, nameof(subject), ThrowError)
                .IsNotNullOrWhitespace(nhsNumber, nameof(nhsNumber), ThrowError)
                .IsValid();

            Subject = subject;
            NhsNumber = nhsNumber;
        }

        public static AccessToken Parse(ILogger logger, string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);
            var nhsNumber = token.Claims.Where(c => string.CompareOrdinal(c.Type, NhsNumberClaimType) == 0)
                .Select(c => c.Value).FirstOrDefault();

            return new AccessToken(logger, token.Subject, nhsNumber);
        }
        
        public static AccessToken Parse(ILogger logger, HttpContext httpContext)
        {
            return new AccessToken(logger, 
                httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), 
                httpContext.User.FindFirstValue(NhsNumberClaimType));
        }
    }
}