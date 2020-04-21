using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.Areas.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.AssertedLoginIdentity
{
    [TestClass]
    public class AssertedLoginIdentityControllerTests
    {
        private AssertedLoginIdentityController _systemUnderTest;

        private IFixture _fixture;
        private Mock<IAssertedLoginIdentityService> _mockAssertedLoginIdentityService;
        private Mock<IAuditor> _mockAuditor;
        private Mock<ILogger<AssertedLoginIdentityController>> _mockLogger;
        private P9UserSession _userSession;
        private CreateJwtRequest _request;

        private const string RequestAuditType = "AssertedLoginIdentity_CreateJwt_Request";
        private const string ResponseAuditType = "AssertedLoginIdentity_CreateJwt_Response";

       [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _fixture.Customize<P9UserSession>(c =>
                c.With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _userSession = _fixture.Create<P9UserSession>();
            _request = _fixture.Create<CreateJwtRequest>();

            _mockAssertedLoginIdentityService = _fixture.Freeze<Mock<IAssertedLoginIdentityService>>();
            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<AssertedLoginIdentityController>>>();

            _systemUnderTest = _fixture.Create<AssertedLoginIdentityController>();
        }

        [TestMethod]
        public async Task Post_ServiceReturnsInternalServerError_Returns500StatusCode()
        {
            // Arrange
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_userSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(_request, _userSession);

            // Assert
            _mockAssertedLoginIdentityService.Verify();

            result.Should().BeOfType<StatusCodeResult>().Subject
                .StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockAuditor.Verify(x => x.Audit(RequestAuditType,
                "Creating Asserted login Identity JWT for Provider ID '{0}', Provider Name '{1}', "
                + "Jump Off ID '{2}', intended relying party URL: {3}",
                _request.ProviderId, _request.ProviderName, _request.JumpOffId,
                _request.IntendedRelyingPartyUrl));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "AssertedLoginIdentity Token creation failed."));

            _mockLogger.VerifyLogger(LogLevel.Information, "Created Asserted Login Identity", Times.Never());
        }

        [TestMethod]
        public async Task Post_ServiceSucceeds_ReturnsCreatedResult()
        {
            // Arrange
            var expectedResponse = _fixture.Create<CreateJwtResponse>();
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_userSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.Success(expectedResponse));

            // Act
            var result = await _systemUnderTest.Post(_request, _userSession);

            // Assert
            _mockAssertedLoginIdentityService.Verify();

            result.Should().BeOfType<CreatedResult>()
                .Subject.Value.Should().BeOfType<CreateJwtResponse>()
                .Subject.Should().Be(expectedResponse);

            _mockAuditor.Verify(x => x.Audit(RequestAuditType,
                "Creating Asserted login Identity JWT for Provider ID '{0}', Provider Name '{1}', "
                + "Jump Off ID '{2}', intended relying party URL: {3}",
                _request.ProviderId, _request.ProviderName, _request.JumpOffId,
                _request.IntendedRelyingPartyUrl));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "AssertedLoginIdentity Token creation succeeded."));

            _mockLogger.VerifyLogger(LogLevel.Information,
                $"Created Asserted Login Identity: ProviderId={_request.ProviderId} " +
                    $"ProviderName={_request.ProviderName} " +
                    $"JumpOffId={_request.JumpOffId} " +
                    $"IntendedRelyingPartyUrl={_request.IntendedRelyingPartyUrl}", Times.Once());
        }
    }
}