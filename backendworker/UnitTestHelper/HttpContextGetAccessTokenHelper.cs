using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTestHelper
{
    public static class HttpContextGetAccessTokenHelper
    {
        public static Mock<HttpContext> CreateMockHttpContext()
        {
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "Name"),
                new Claim("nhs_number", "NHS Number")
            });

            var mockHttpContext = new Mock<HttpContext>();
            var claimsPrincipal = new ClaimsPrincipal(identity);
            mockHttpContext.Setup(x => x.User)
                .Returns(claimsPrincipal);
            
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest
                .Setup(x => x.Headers)
                .Returns(new HeaderDictionary { { "Authorization", "Auth" } });

            mockHttpContext
                .Setup(x => x.Request)
                .Returns(mockHttpRequest.Object);

            return mockHttpContext;
        }
    }
}