using System.Collections.Generic;
using System.Security.Claims;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTestHelper
{
    public static class HttpContextGetAccessTokenHelper
    {
        public static Mock<HttpContext> CreateMockHttpContext(IFixture fixture)
        {
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, fixture.Create<string>()),
                new Claim("nhs_number", fixture.Create<string>())
            });

            var mockHttpContext = fixture.Create<Mock<HttpContext>>();
            mockHttpContext.Setup(x => x.User)
                .Returns(new ClaimsPrincipal(identity));
            return mockHttpContext;
        }
    }
}