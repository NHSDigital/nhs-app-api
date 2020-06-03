using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;

namespace UnitTestHelper
{
    public static class AccessTokenMock
    {
        public static AccessToken Generate(string subject = "NhsLoginId", string nhsNumber = "NhsNumber")
        {
            var rawToken = JwtToken.Generate(new Claim[]
            {
                new Claim("sub", subject),
                new Claim("nhs_number", nhsNumber),
            });

            return AccessToken.Parse(new Mock<ILogger>().Object, rawToken);
        }
    }
}