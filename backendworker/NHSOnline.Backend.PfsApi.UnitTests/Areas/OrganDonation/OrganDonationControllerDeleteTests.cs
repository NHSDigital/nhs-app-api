using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.Areas.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.OrganDonation
{
    [TestClass]
    public sealed class OrganDonationControllerDeleteTests : IDisposable
    {
        private OrganDonationController _systemUnderTest;
        private Mock<IOrganDonationService> _mockOrganDonationService;
        private P9UserSession _userSession;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IOrganDonationValidationService> _mockValidator;

        private const string RequestAuditType = "OrganDonation_Withdraw_Request";
        private const string ResponseAuditType = "OrganDonation_Withdraw_Response";

        private const string RequestAuditMessage = "Attempting to withdraw organ donation decision";

        [TestInitialize]
        public void TestInitialize()
        {
            _userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token");
            _mockOrganDonationService = new Mock<IOrganDonationService>();
            _mockAuditor = new Mock<IAuditor>();
            
            _mockValidator = new Mock<IOrganDonationValidationService>();
            _mockValidator
                .Setup(x => x.IsDeleteValid(It.IsAny<OrganDonationWithdrawRequest>()))
                .Returns(true);

            _systemUnderTest = new OrganDonationController(
                new Mock<ILogger<OrganDonationController>>().Object,
                new Mock<IGpSystemFactory>().Object,
                _mockOrganDonationService.Object,
                _mockAuditor.Object,
                _mockValidator.Object);
        }

        [TestMethod]
        public async Task Delete_WhenServiceReturnsSuccessResult_ReturnsSuccessfulResult()
        {
            // Arrange
            var newResult = new OrganDonationWithdrawResult.SuccessfullyWithdrawn();
            
            _mockOrganDonationService
                .Setup(x => x.Withdraw(It.IsAny<OrganDonationWithdrawRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationWithdrawResult) newResult));

            // Act
            var result = await _systemUnderTest.Delete(new OrganDonationWithdrawRequest(), _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status200OK);
            _mockOrganDonationService.Verify(x => x.Withdraw(It.IsAny<OrganDonationWithdrawRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "The organ donation decision has been successfully Withdrawn"));
        }

        [TestMethod]
        public async Task Delete_WhenValidationFails_ReturnsBadRequest()
        {
            // Arrange
            _mockValidator
                .Setup(x => x.IsDeleteValid(It.IsAny<OrganDonationWithdrawRequest>()))
                .Returns(false);
            
            // Act
            var result = await _systemUnderTest.Delete(new OrganDonationWithdrawRequest(), _userSession);
            
            // Assert
            result.Should().BeOfType<BadRequestResult>();
            _mockOrganDonationService.Verify(x => x.Withdraw(It.IsAny<OrganDonationWithdrawRequest>(), _userSession), Times.Never);
            
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "The organ donation withdraw request failed validation"));
        }

        [TestMethod]
        public async Task Delete_WhenServiceReturnTimeoutResult_ReturnsBadGateway()
        {
            // Arrange
            var timeoutResult = new OrganDonationWithdrawResult.Timeout();

            _mockOrganDonationService.Setup(x => x.Withdraw(It.IsAny<OrganDonationWithdrawRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationWithdrawResult) timeoutResult));

            // Act
            var result = await _systemUnderTest.Delete(new OrganDonationWithdrawRequest(), _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);

            _mockOrganDonationService.Verify(x => x.Withdraw(It.IsAny<OrganDonationWithdrawRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "The organ donation Withdraw system took too long to respond"));
        }

        [TestMethod]
        public async Task Delete_WhenServiceReturnUpstreamErrorResult_ReturnsBadGateway()
        {
            // Arrange
            var response = new PfsErrorResponse();
            var upstreamErrorResult = new OrganDonationWithdrawResult.UpstreamError(response);

            _mockOrganDonationService
                .Setup(x => x.Withdraw(It.IsAny<OrganDonationWithdrawRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationWithdrawResult) upstreamErrorResult));

            // Act
            var result = await _systemUnderTest.Delete(new OrganDonationWithdrawRequest(), _userSession);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            statusCodeResult.Value.Should().Be(response);

            _mockOrganDonationService.Verify(x => x.Withdraw(It.IsAny<OrganDonationWithdrawRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => 
                x.Audit(ResponseAuditType, "There was an upstream error when withdrawing the organ donation decision"));
        }
        
        [TestMethod]
        public async Task Delete_WhenServiceReturnSystemErrorResult_ReturnsInternalServerError()
        {
            // Arrange
            var systemErrorResult = new OrganDonationWithdrawResult.SystemError();

            _mockOrganDonationService
                .Setup(x => x.Withdraw(It.IsAny<OrganDonationWithdrawRequest>(), _userSession))
                .Returns(Task.FromResult((OrganDonationWithdrawResult) systemErrorResult));

            // Act
            var result = await _systemUnderTest.Delete(new OrganDonationWithdrawRequest(), _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockOrganDonationService.Verify(x => x.Withdraw(It.IsAny<OrganDonationWithdrawRequest>(), _userSession));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(
                x => x.Audit(ResponseAuditType, "There was an issue withdrawing the organ donation decision"));
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}