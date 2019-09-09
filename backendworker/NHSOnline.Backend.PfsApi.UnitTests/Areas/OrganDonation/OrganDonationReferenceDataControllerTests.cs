using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.OrganDonation
{
    [TestClass]
    public class OrganDonationReferenceDataControllerTests
    {
        private OrganDonationReferenceDataController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IOrganDonationService> _mockOrganDonationService;
        private UserSession _userSession;
        private Mock<IAuditor> _mockAuditor;

        private const string RequestAuditType = "OrganDonation_ReferenceData_Request";
        private const string ResponseAuditType = "OrganDonation_ReferenceData_Response";

        private const string RequestAuditMessage = "Attempting to get organ donation reference data";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _userSession = _fixture.Create<UserSession>();
            _mockOrganDonationService = _fixture.Freeze<Mock<IOrganDonationService>>();
            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_userSession);

            _systemUnderTest = _fixture.Create<OrganDonationReferenceDataController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessullyRetrievedResult()
        {
            // Arrange
            var organDonationReferenceDataResponse = _fixture.Create<OrganDonationReferenceDataResponse>();
            var newResult = new OrganDonationReferenceDataResult.SuccessfullyRetrieved(organDonationReferenceDataResponse);
            
            _mockOrganDonationService
                .Setup(x => x.GetReferenceData())
                .Returns(Task.FromResult((OrganDonationReferenceDataResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(organDonationReferenceDataResponse);
            
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "The organ donation reference data has been retrieved successfully"));
        }

        [TestMethod]
        public async Task Get_ReturnsGatewayTimeout_WhenServiceReturnTimeoutResult()
        {
            // Arrange
            var newResult = new OrganDonationReferenceDataResult.Timeout();

            _mockOrganDonationService
                .Setup(x => x.GetReferenceData())
                .Returns(Task.FromResult((OrganDonationReferenceDataResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status504GatewayTimeout);

            _mockOrganDonationService.Verify(x => x.GetReferenceData());
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "The organ donation reference data system took too long to respond"));
        }

        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenServiceReturnUpstreamErrorResult()
        {
            // Arrange
            var response = _fixture.Create<PfsErrorResponse>();
            var newResult = new OrganDonationReferenceDataResult.UpstreamError(response);

            _mockOrganDonationService
                .Setup(x => x.GetReferenceData())
                .Returns(Task.FromResult((OrganDonationReferenceDataResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
                statusCodeResult.Value.Should().Be(response);
            }

            _mockOrganDonationService.Verify(x => x.GetReferenceData());
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => 
                x.Audit(ResponseAuditType, "There was an upstream error when getting the organ donation reference data"));
        }
        
        [TestMethod]
        public async Task Get_ReturnsInternalServerError_WhenServiceReturnSystemErrorResult()
        {
            // Arrange
            var newResult = new OrganDonationReferenceDataResult.SystemError();

            _mockOrganDonationService
                .Setup(x => x.GetReferenceData())
                .Returns(Task.FromResult((OrganDonationReferenceDataResult) newResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockOrganDonationService.Verify(x => x.GetReferenceData());
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(
                x => x.Audit(ResponseAuditType, "There was an issue getting the organ donation reference data"));
        }
    }
}