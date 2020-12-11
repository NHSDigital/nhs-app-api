using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.Areas.LinkedAccounts;
using NHSOnline.Backend.PfsApi.Areas.LinkedAccounts.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.Support.Session;
using Constants = NHSOnline.Backend.Support.Constants;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.LinkedAccounts
{
    [TestClass]
    public sealed class LinkedAccountControllerTests : IDisposable
    {
        private LinkedAccountsController _systemUnderTest;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<ILinkedAccountsService> _linkedAccountService;
        private Mock<IGpSearchService> _gpSearchService;
        private P9UserSession _userSession;
        private GpUserSession _gpUserSession;
        private LinkedAccountAuditInfo _linkedAccountAuditInfo;
        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<IAuditor> _auditor;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _gpSearchService = new Mock<IGpSearchService>();
            _mockSessionCacheService = new Mock<ISessionCacheService>();
            _auditor = new Mock<IAuditor>();
            _linkedAccountService = new Mock<ILinkedAccountsService>();
            _gpUserSession = new MockGpUserSession();
            _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(), new EmisUserSession(), "im1token");
            _linkedAccountAuditInfo = new LinkedAccountAuditInfo();

            var mockGpSystem = new Mock<IGpSystem>();
            mockGpSystem.Setup(x => x.GetLinkedAccountsService())
                .Returns(_linkedAccountService.Object);

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_gpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.LinkedAccountAuditInfo, _linkedAccountAuditInfo }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

            _systemUnderTest = new LinkedAccountsController(new Mock<ILogger<LinkedAccountsController>>().Object,
                _mockGpSystemFactory.Object,
                _gpSearchService.Object,
                _mockSessionCacheService.Object,
                _auditor.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object }
            };

        }

        [TestMethod]
        public async Task SwitchBackToMainAccount_Returns200_WhenLinkedAccountIdIsFound()
        {
            // Arrange
            _linkedAccountAuditInfo.IsProxyMode = true;
            _userSession.GpUserSession.NhsNumber = "123 456 789";

            var id = Guid.NewGuid();

            _linkedAccountService.Setup(x => x.SwitchAccount(
                    It.Is<GpLinkedAccountModel>(gp => gp.GpUserSession == _gpUserSession && gp.PatientId == id)))
                .ReturnsAsync(new SwitchAccountResult.Success())
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Switch(id, _gpUserSession);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_gpUserSession.Supplier));
            _linkedAccountService.Verify();
            result.Should().BeAssignableTo<OkResult>();
            _auditor.Verify(x => x.Audit(AuditingOperations.LinkedAccountsSwitchResponse, It.IsAny<string>()));
        }


        [TestMethod]
        public async Task SwitchToProxy_Returns200_WhenLinkedAccountIdIsFound()
        {
            // Arrange
            _linkedAccountAuditInfo.IsProxyMode = false;
            _userSession.GpUserSession.NhsNumber = "123 456 789";

            var id = Guid.NewGuid();

            _linkedAccountService.Setup(x => x.SwitchAccount(
                    It.Is<GpLinkedAccountModel>(gp => gp.GpUserSession == _gpUserSession && gp.PatientId == id)))
                .ReturnsAsync(new SwitchAccountResult.Success())
                .Verifiable();


            _linkedAccountService.Setup(x => x.GetNhsNumberForProxyUser(_gpUserSession, id))
                .Returns("123")
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Switch(id, _gpUserSession);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_gpUserSession.Supplier));
            _linkedAccountService.Verify();
            result.Should().BeAssignableTo<OkResult>();
            _auditor.Verify(x => x.Audit(AuditingOperations.LinkedAccountsSwitchResponse, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task Switch_Returns404_WhenLinkedAccountIdIsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _linkedAccountService.Setup(x => x.SwitchAccount(
                    It.Is<GpLinkedAccountModel>(gp => gp.GpUserSession == _gpUserSession && gp.PatientId == id)))
                .ReturnsAsync(new SwitchAccountResult.Failure(id))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Switch(id, _gpUserSession);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_gpUserSession.Supplier));
            _linkedAccountService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            _auditor.Verify(x => x.Audit(AuditingOperations.LinkedAccountsSwitchResponse, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task Switch_Returns200_WhenLinkedAccountIdAlreadyAuthenticated()
        {
            // Arrange
            var id = Guid.NewGuid();

            _linkedAccountService.Setup(x => x.SwitchAccount(
                    It.Is<GpLinkedAccountModel>(gp => gp.GpUserSession == _gpUserSession && gp.PatientId == id)))
                .ReturnsAsync(new SwitchAccountResult.AlreadyAuthenticated(id))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Switch(id, _gpUserSession);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_gpUserSession.Supplier));
            _linkedAccountService.Verify();
            result.Should().BeAssignableTo<OkResult>();
            _auditor.Verify(x => x.Audit(AuditingOperations.LinkedAccountsSwitchResponse, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResultAndUserSessionUpdated_WhenServiceReturnsSuccessfully()
        {
            LinkedAccountsResult linkedAccountResult = new LinkedAccountsResult.Success(
                new List<LinkedAccount>(),
                true);

            // Arrange
            _linkedAccountService.Setup(x => x.GetLinkedAccounts(_gpUserSession))
                .ReturnsAsync(linkedAccountResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_gpUserSession, _userSession);

            // Assert
            _mockSessionCacheService.Verify(x => x.UpdateUserSession(_userSession));
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_gpUserSession.Supplier));
            _linkedAccountService.VerifyAll();

            var subject = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            subject.StatusCode.Value.Should().Equals(HttpStatusCode.OK);
            subject.Value.Should().NotBeNull();
        }


        [TestMethod]
        public async Task Get_ReturnsSuccessfulResultAndUserSessionNotUpdated_WhenServiceReturnsSuccessfully()
        {
            LinkedAccountsResult linkedAccountResult = new LinkedAccountsResult.Success(
                new List<LinkedAccount>(),
                false);

            // Arrange
            _linkedAccountService.Setup(x => x.GetLinkedAccounts(_gpUserSession))
                .ReturnsAsync(linkedAccountResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_gpUserSession, _userSession);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_gpUserSession.Supplier));
            _mockSessionCacheService.Verify(x => x.UpdateUserSession(It.IsAny<P9UserSession>()), Times.Never());

            var subject = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            subject.StatusCode.Value.Should().Equals(HttpStatusCode.OK);
            subject.Value.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetAccessSummaryOfLinkedAccount_ReturnsSuccessfulResult_WhenServicesReturnsSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var odsCode = "ODS code";
            var organisationName = "Org name";
            var gpSearchResponse = new GpSearchResponse
            {
                Organisations = new List<Organisation>
                {
                    new Organisation
                    {
                        OrganisationName = organisationName,
                    }
                }
            };
            var gpSearchResult = new GpSearchResult.Success(gpSearchResponse);

            var linkedAccountAccessSummaryResponse = new GetAccountAccessSummaryResponse();
            LinkedAccountAccessSummaryResult linkedAccountSummaryResult = new LinkedAccountAccessSummaryResult.Success(linkedAccountAccessSummaryResponse);

            _linkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_gpUserSession, id))
                .Returns(odsCode)
                .Verifiable();

            _linkedAccountService.Setup(x => x.GetLinkedAccount(_gpUserSession, id))
                .ReturnsAsync(linkedAccountSummaryResult)
                .Verifiable();

            _gpSearchService.Setup(x => x.GetGpPracticeByOdsCode(odsCode))
                .ReturnsAsync(gpSearchResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAccessSummaryOfLinkedAccount(id, _gpUserSession);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_gpUserSession.Supplier));
            _linkedAccountService.Verify();
            _gpSearchService.Verify();
            var subject = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            subject.StatusCode.Value.Should().Equals(HttpStatusCode.OK);
            subject.Value.Should().NotBeNull();
            var response = subject.Value.Should().BeAssignableTo<LinkedAccountAccessSummaryResponse>().Subject;

            response.GpPracticeName.Should().Be(organisationName);
            response.CanBookAppointment.Should().Be(linkedAccountAccessSummaryResponse.CanBookAppointment);
            response.CanOrderRepeatPrescription.Should().Be(linkedAccountAccessSummaryResponse.CanOrderRepeatPrescription);
            response.CanViewMedicalRecord.Should().Be(linkedAccountAccessSummaryResponse.CanViewMedicalRecord);
        }

        [TestMethod]
        public async Task GetAccessSummaryOfLinkedAccount_SetsPracticeNameToOdsCode_WhenGpSearchServiceDoesNotReturnAPractice()
        {
            // Arrange
            var id = Guid.NewGuid();
            var odsCode = "ODS code";
            var gpSearchResponse = new GpSearchResponse();
            var gpSearchResult = new GpSearchResult.Success(gpSearchResponse);

            var linkedAccountAccessSummaryResponse = new GetAccountAccessSummaryResponse();
            LinkedAccountAccessSummaryResult linkedAccountSummaryResult = new LinkedAccountAccessSummaryResult.Success(linkedAccountAccessSummaryResponse);

            _linkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_gpUserSession, id))
                .Returns(odsCode)
                .Verifiable();

            _linkedAccountService.Setup(x => x.GetLinkedAccount(_gpUserSession, id))
                .ReturnsAsync(linkedAccountSummaryResult)
                .Verifiable();

            _gpSearchService.Setup(x => x.GetGpPracticeByOdsCode(odsCode))
                .ReturnsAsync(gpSearchResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAccessSummaryOfLinkedAccount(id, _gpUserSession);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_gpUserSession.Supplier));
            _linkedAccountService.Verify();
            _gpSearchService.Verify();
            var subject = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            subject.StatusCode.Value.Should().Equals(HttpStatusCode.OK);
            subject.Value.Should().NotBeNull();
            var response = subject.Value.Should().BeAssignableTo<LinkedAccountAccessSummaryResponse>().Subject;

            response.GpPracticeName.Should().Be(odsCode);
            response.CanBookAppointment.Should().Be(linkedAccountAccessSummaryResponse.CanBookAppointment);
            response.CanOrderRepeatPrescription.Should().Be(linkedAccountAccessSummaryResponse.CanOrderRepeatPrescription);
            response.CanViewMedicalRecord.Should().Be(linkedAccountAccessSummaryResponse.CanViewMedicalRecord);
        }

        [TestMethod]
        public async Task GetAccessSummaryOfLinkedAccount_ResultsInEmptyPracticeNameTo_WhenGpSearchServiceErrors()
        {
            // Arrange
            var id = Guid.NewGuid();
            var odsCode = "ODS code";
            var gpSearchResult = new GpSearchResult.InternalServerError();

            var linkedAccountAccessSummaryResponse = new GetAccountAccessSummaryResponse();
            LinkedAccountAccessSummaryResult linkedAccountSummaryResult = new LinkedAccountAccessSummaryResult.Success(linkedAccountAccessSummaryResponse);

            _linkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_gpUserSession, id))
                .Returns(odsCode)
                .Verifiable();

            _linkedAccountService.Setup(x => x.GetLinkedAccount(_gpUserSession, id))
                .ReturnsAsync(linkedAccountSummaryResult)
                .Verifiable();

            _gpSearchService.Setup(x => x.GetGpPracticeByOdsCode(odsCode))
                .ReturnsAsync(gpSearchResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAccessSummaryOfLinkedAccount(id, _gpUserSession);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_gpUserSession.Supplier));
            _linkedAccountService.Verify();
            _gpSearchService.Verify();
            var subject = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            subject.StatusCode.Value.Should().Equals(HttpStatusCode.OK);
            subject.Value.Should().NotBeNull();
            var response = subject.Value.Should().BeAssignableTo<LinkedAccountAccessSummaryResponse>().Subject;

            response.GpPracticeName.Should().BeEmpty();
            response.CanBookAppointment.Should().Be(linkedAccountAccessSummaryResponse.CanBookAppointment);
            response.CanOrderRepeatPrescription.Should().Be(linkedAccountAccessSummaryResponse.CanOrderRepeatPrescription);
            response.CanViewMedicalRecord.Should().Be(linkedAccountAccessSummaryResponse.CanViewMedicalRecord);
        }

        [TestMethod]
        public async Task GetAccessSummaryOfLinkedAccount_Returns502_WhenLinkedAccountServiceReturnsErrorResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var odsCode = "ODS code";
            var gpSearchResponse = new GpSearchResponse();
            var gpSearchResult = new GpSearchResult.Success(gpSearchResponse);

            LinkedAccountAccessSummaryResult linkedAccountSummaryResult = new LinkedAccountAccessSummaryResult.BadGateway();

            _linkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_gpUserSession, id))
                .Returns(odsCode)
                .Verifiable();

            _linkedAccountService.Setup(x => x.GetLinkedAccount(_gpUserSession, id))
                .ReturnsAsync(linkedAccountSummaryResult)
                .Verifiable();

            _gpSearchService.Setup(x => x.GetGpPracticeByOdsCode(odsCode))
                .ReturnsAsync(gpSearchResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAccessSummaryOfLinkedAccount(id, _gpUserSession);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_gpUserSession.Supplier));
            _linkedAccountService.Verify();
            _gpSearchService.Verify();
            var subject = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}
