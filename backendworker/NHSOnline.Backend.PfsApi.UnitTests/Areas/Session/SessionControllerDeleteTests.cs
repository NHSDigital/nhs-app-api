using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.UnitTests.Audit;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public sealed class SessionControllerDeleteTests
    {
        private SessionControllerTestContext Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new SessionControllerTestContext();
        }

        [TestMethod]
        public async Task Delete_P9UserSessionSucceeds_Returns204NoContent()
        {
            // Arrange
            var userSession = CreateP9UserSession(supplier: Supplier.Emis, nhsNumber: "123 456 7890", accessToken: "AccToken", key: "SessionKey");
            ArrangeCloseAndDeleteGpUserSession(userSession.GpUserSession, new CloseSessionResult.Success());
            ArrangeDeleteCachedUserSession("SessionKey");
            var auditStub = ArrangeAudit();

            // Act
            var result = await CreateSystemUnderTest().Delete(userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status204NoContent);

            using (new AssertionScope())
            {
                auditStub.AccessTokenString.Should().Be("AccToken");
                auditStub.NhsNumber.Should().Be("123 456 7890");
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("Session_Delete");
                auditStub.Details.Should().Be("Session delete called");
                auditStub.ResponseDetails.Should().Be("Session successfully deleted");
            }
        }

        [TestMethod]
        public async Task Delete_P5UserSessionSucceeds_Returns204NoContent()
        {
            // Arrange
            var userSession = CreateP5UserSession(key: "SessionKey");
            ArrangeDeleteCachedUserSession("SessionKey");

            // Act
            var result = await CreateSystemUnderTest().Delete(userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [TestMethod]
        public async Task Delete_P9UserSessionDeleteGpUserSessionFails_Returns500InternalServiceError()
        {
            // Arrange
            var userSession = CreateP9UserSession(supplier: Supplier.Emis, nhsNumber: "123 456 7890", accessToken: "AccToken", key: "SessionKey");
            ArrangeCloseAndDeleteGpUserSession(userSession.GpUserSession, new CloseSessionResult.Failure());
            ArrangeDeleteCachedUserSession("SessionKey");
            var auditStub = ArrangeAudit();

            // Act
            var result = await CreateSystemUnderTest().Delete(userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            using (new AssertionScope())
            {
                auditStub.AccessTokenString.Should().Be("AccToken");
                auditStub.NhsNumber.Should().Be("123 456 7890");
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("Session_Delete");
                auditStub.Details.Should().Be("Session delete called");
                auditStub.ResponseDetails.Should().Be("Delete session failed");
            }
        }

        [TestMethod]
        public async Task Delete_P5UserSession_DeleteGpUserSessionNotCalled()
        {
            // Arrange
            var userSession = CreateP5UserSession(key: "SessionKey");
            ArrangeDeleteCachedUserSession("SessionKey");

            // Act
            await CreateSystemUnderTest().Delete(userSession);

            // Assert
            Context.MockGpSessionManager.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Delete_P9UserSessionDeleteSessionCacheFails_Returns500InternalServiceError()
        {
            // Arrange
            var userSession = CreateP9UserSession(supplier: Supplier.Emis, nhsNumber: "123 456 7890", accessToken: "AccToken", key: "SessionKey");
            ArrangeCloseAndDeleteGpUserSession(userSession.GpUserSession, new CloseSessionResult.Success());
            ArrangeDeleteCachedUserSession("SessionKey", false);
            var auditStub = ArrangeAudit();

            // Act
            var result = await CreateSystemUnderTest().Delete(userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            using (new AssertionScope())
            {
                auditStub.AccessTokenString.Should().Be("AccToken");
                auditStub.NhsNumber.Should().Be("123 456 7890");
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("Session_Delete");
                auditStub.Details.Should().Be("Session delete called");
                auditStub.ResponseDetails.Should().Be("Delete session failed");
            }
        }

        [TestMethod]
        public async Task Delete_P5UserSessionDeleteSessionCacheFails_Returns500InternalServiceError()
        {
            // Arrange
            var userSession = CreateP5UserSession(key: "SessionKey");
            ArrangeDeleteCachedUserSession("SessionKey", false);

            // Act
            var result = await CreateSystemUnderTest().Delete(userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        }

        [TestMethod]
        public async Task Delete_P5UserSession_AuditNotCalled()
        {
            // Arrange
            var userSession = CreateP5UserSession(key: "SessionKey");
            ArrangeDeleteCachedUserSession("SessionKey");

            // Act
            await CreateSystemUnderTest().Delete(userSession);

            // Assert
            Context.MockAuditor.VerifyNoOtherCalls();
        }

        private P9UserSession CreateP9UserSession(
            string nhsNumber = null,
            string accessToken = null,
            string key = null,
            Supplier? supplier = null)
        {
            var mockGpUserSession = new Mock<GpUserSession>();
            mockGpUserSession.Setup(x => x.Supplier).Returns(supplier ?? Supplier.Unknown);
            mockGpUserSession.Object.NhsNumber = nhsNumber ?? "NHS Number";
            var citizenIdUserSession = new CitizenIdUserSession { AccessToken =  accessToken ?? "Access Token"};

            return new P9UserSession("CSRF", citizenIdUserSession, mockGpUserSession.Object, "Im1")
                { Key = key ?? "Key" };
        }

        private P5UserSession CreateP5UserSession(string key = null)
        {
            var citizenIdUserSession = new CitizenIdUserSession();

            return new P5UserSession("CSRF", citizenIdUserSession) { Key = key ?? "Key" };
        }

        private void ArrangeDeleteCachedUserSession(string key, bool? result = null)
        {
            Context.MockSessionCacheService
                .Setup(x => x.DeleteUserSession(key))
                .ReturnsAsync(result ?? true);
        }

        private void ArrangeCloseAndDeleteGpUserSession(GpUserSession gpUserSession, CloseSessionResult result = null)
        {
            Context.MockGpSessionManager
                .Setup(x => x.CloseSession(gpUserSession))
                .ReturnsAsync(result ?? new CloseSessionResult.Success());
        }

        private AuditBuilderStub ArrangeAudit() => Context.ArrangeAudit();

        private SessionController CreateSystemUnderTest() => Context.CreateSystemUnderTest();
    }
}