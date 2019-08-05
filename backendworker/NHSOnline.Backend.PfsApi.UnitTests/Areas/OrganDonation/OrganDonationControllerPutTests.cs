using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.OrganDonation
{
    [TestClass]
    public class OrganDonationControllerPutTests
    {
        private OrganDonationController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IOrganDonationService> _mockOrganDonationService;
        private UserSession _userSession;
        private Mock<IAuditor> _mockAuditor;

        private const string RequestAuditType = "OrganDonation_Update_Request";
        private const string ResponseAuditType = "OrganDonation_Update_Response";

        private const string RequestAuditMessage = "Attempting to update organ donation decision";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            var mockValidator = _fixture.Freeze<Mock<IOrganDonationValidationService>>();
            mockValidator
                .Setup(x => x.IsPutValid(It.IsAny<OrganDonationRegistrationRequest>()))
                .Returns(true);

            _userSession = _fixture.Create<UserSession>();
            _mockOrganDonationService = _fixture.Freeze<Mock<IOrganDonationService>>();
            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_userSession);


            _systemUnderTest = _fixture.Create<OrganDonationController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Put_ReturnsSuccessfulResult_WhenServiceReturnsSuccessResult()
        {
            // Arrange
            var organDonationRegistrationResponse = _fixture.Create<OrganDonationRegistrationResponse>();
            var newResult = new OrganDonationRegistrationResult.SuccessfullyRegistered(organDonationRegistrationResponse);
            
            _mockOrganDonationService
                .Setup(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) newResult));

            // Act
            var result = await _systemUnderTest.Put(new OrganDonationRegistrationRequest());

            // Assert
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(organDonationRegistrationResponse);
            _mockOrganDonationService.Verify(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "The organ donation decision has been successfully updated"));
        }

        [TestMethod]
        public async Task Put_ReturnsGatewayTimeout_WhenServiceReturnTimeoutResult()
        {
            // Arrange
            var timeoutResult = new OrganDonationRegistrationResult.Timeout();

            _mockOrganDonationService.Setup(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) timeoutResult));

            // Act
            var result = await _systemUnderTest.Put(new OrganDonationRegistrationRequest());

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status504GatewayTimeout);

            _mockOrganDonationService.Verify(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "The organ donation registration update system took too long to respond"));
        }

        [TestMethod]
        public async Task Put_ReturnsBadGateway_WhenServiceReturnUpstreamErrorResult()
        {
            // Arrange
            var response = _fixture.Create<ApiErrorResponse>();
            var upstreamErrorResult = new OrganDonationRegistrationResult.UpstreamError(response);

            _mockOrganDonationService
                .Setup(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) upstreamErrorResult));

            // Act
            var result = await _systemUnderTest.Put(new OrganDonationRegistrationRequest());

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            statusCodeResult.Value.Should().Be(response);

            _mockOrganDonationService.Verify(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => 
                x.Audit(ResponseAuditType, "There was an upstream error when registering the organ donation decision update"));
        }
        
        [TestMethod]
        public async Task Put_ReturnsInternalServerError_WhenServiceReturnSystemErrorResult()
        {
            // Arrange
            var systemErrorResult = new OrganDonationRegistrationResult.SystemError();

            _mockOrganDonationService
                .Setup(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationRegistrationResult) systemErrorResult));

            // Act
            var result = await _systemUnderTest.Put(new OrganDonationRegistrationRequest());

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockOrganDonationService.Verify(x => x.Update(It.IsAny<OrganDonationRegistrationRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(
                x => x.Audit(ResponseAuditType, "There was an issue registering the organ donation decision update"));
        }
    }
}