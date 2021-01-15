using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionCreateResultVisitorTests
    {
        private SessionCreateResultVisitorTestContext Context { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Context = new SessionCreateResultVisitorTestContext();
        }

        [TestMethod]
        public async Task Visit_WhenSuccessResult_SetsSessionExpiryCookie()
        {
            // Arrange
            var expectedJwtToken = "Jwt.To.Ken";
            var expectedCookieOptions = new CookieOptions()
            {
                Secure = false,
                SameSite = SameSiteMode.Lax,
                HttpOnly = true,
                Domain = SessionCreateResultVisitorTestContext.CookieDomain
            };

            // Act
            await Context.CreateSystemUnderTest().Visit(Context.Data.SuccessResult, Context.Mocks.HttpContext.Object, expectedJwtToken, "");

            // Assert
            Context.Mocks.ResponseCookies
                .Verify(r => r.Append(
                    "NHSO-Session-Expiry",
                    expectedJwtToken,
                    It.Is<CookieOptions>(c =>
                        expectedCookieOptions.Domain == c.Domain &&
                        expectedCookieOptions.SameSite == c.SameSite &&
                        expectedCookieOptions.HttpOnly &&
                        !expectedCookieOptions.Secure)), Times.Once);
        }
    }
}