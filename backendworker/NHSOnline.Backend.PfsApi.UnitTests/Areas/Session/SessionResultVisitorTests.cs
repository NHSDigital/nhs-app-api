using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionResultVisitorTests
    {
        [TestMethod]
        public async Task Visit_P9_WhenSuccessResult_SetsSessionExpiryCookie()
        {
            // Arrange
            SessionResultVisitorTestContext context = new SessionResultVisitorTestContext();
            var expectedJwtToken = "Jwt.To.Ken";
            var expectedCookieOptions = new CookieOptions()
            {
                Secure = false,
                SameSite = SameSiteMode.Lax,
                HttpOnly = true,
                Domain = SessionResultVisitorTestContext.CookieDomain
            };

            // Act
            await context.CreateSystemUnderTest().Visit(context.Data.SuccessResult, context.Mocks.HttpContext.Object, expectedJwtToken, "referrer", "integrationReferrer", "referrerOrigin");

            // Assert
            context.Mocks.ResponseCookies
                .Verify(r => r.Append(
                    "NHSO-Session-Expiry",
                    expectedJwtToken,
                    It.Is<CookieOptions>(c =>
                        expectedCookieOptions.Domain == c.Domain &&
                        expectedCookieOptions.SameSite == c.SameSite &&
                        expectedCookieOptions.HttpOnly &&
                        !expectedCookieOptions.Secure)), Times.Once);
        }

        [TestMethod]
        public async Task Visit_P9_PostOperationAuditSessionEvent_IsBeingCalled()
        {
            // Arrange
            SessionResultVisitorTestContext context = new SessionResultVisitorTestContext();
            var expectedJwtToken = "Jwt.To.Ken";

            // Act
            await context.CreateSystemUnderTest().Visit(context.Data.SuccessResult, context.Mocks.HttpContext.Object, expectedJwtToken, "referrer", "integrationReferrer", "referrerOrigin");

            // Assert
            context.Mocks.Auditor.Verify(x => x.PostOperationAuditSessionEvent(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Supplier>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>()));
        }

        [TestMethod]
        public async Task Visit_P5_WhenSuccessResult_SetsSessionExpiryCookie()
        {
            // Arrange
            SessionResultVisitorTestContext context = new SessionResultVisitorTestContext(ProofLevel.P5, null);
            var expectedJwtToken = "Jwt.To.Ken";
            var expectedCookieOptions = new CookieOptions()
            {
                Secure = false,
                SameSite = SameSiteMode.Lax,
                HttpOnly = true,
                Domain = SessionResultVisitorTestContext.CookieDomain
            };

            // Act
            await context.CreateSystemUnderTest().Visit(context.Data.SuccessResult, context.Mocks.HttpContext.Object, expectedJwtToken, "referrer", "integrationReferrer", "referrerOrigin");

            // Assert
            context.Mocks.ResponseCookies
                .Verify(r => r.Append(
                    "NHSO-Session-Expiry",
                    expectedJwtToken,
                    It.Is<CookieOptions>(c =>
                        expectedCookieOptions.Domain == c.Domain &&
                        expectedCookieOptions.SameSite == c.SameSite &&
                        expectedCookieOptions.HttpOnly &&
                        !expectedCookieOptions.Secure)), Times.Once);
        }

        [TestMethod]
        public async Task Visit_P5_PostOperationAuditSessionEvent_IsBeingCalled()
        {
            // Arrange
            SessionResultVisitorTestContext context = new SessionResultVisitorTestContext(ProofLevel.P5, null);
            var expectedJwtToken = "Jwt.To.Ken";

            // Act
            await context.CreateSystemUnderTest().Visit(context.Data.SuccessResult, context.Mocks.HttpContext.Object, expectedJwtToken, "referrer", "integrationReferrer", "referrerOrigin");

            // Assert
            context.Mocks.Auditor.Verify(x => x.PostOperationAuditSessionEvent(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Supplier>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>()));
        }
    }
}
