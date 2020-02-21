using System.Collections.Generic;
using System.Security.Claims;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
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
            var claimsPrincipal = new ClaimsPrincipal(identity);
            mockHttpContext.Setup(x => x.User)
                .Returns(claimsPrincipal);
            
            var mockHttpRequest = fixture.Create<Mock<HttpRequest>>();
            mockHttpRequest
                .Setup(x => x.Headers)
                .Returns(new HeaderDictionary { { "Authorization", fixture.Create<StringValues>() } });

            mockHttpContext
                .Setup(x => x.Request)
                .Returns(mockHttpRequest.Object);

            return mockHttpContext;
        }
    }
}