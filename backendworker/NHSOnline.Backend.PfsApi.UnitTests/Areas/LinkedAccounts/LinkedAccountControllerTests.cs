using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.PfsApi.Areas.LinkedAccounts;
using NHSOnline.Backend.Support;
using UnitTestHelper;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using Constants = NHSOnline.Backend.Support.Constants;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.LinkedAccounts
{
    [TestClass]
    public class LinkedAccountControllerTests
    {
        private LinkedAccountsController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<ILinkedAccountsService> _linkedAccountService;
        private Mock<IGpSearchService> _gpSearchService;
        private UserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _gpSearchService = _fixture.Freeze<Mock<IGpSearchService>>();
            _linkedAccountService = new Mock<ILinkedAccountsService>();
            _userSession = _fixture.Create<UserSession>();

            var mockGpSystem = new Mock<IGpSystem>();
            mockGpSystem.Setup(x => x.GetLinkedAccountsService())
                .Returns(_linkedAccountService.Object);

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

            _systemUnderTest = _fixture.Create<LinkedAccountsController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            LinkedAccountsResult linkedAccountResult = new LinkedAccountsResult.Success(
                _fixture.Create<GetLinkedAccountsResponse>());

            // Arrange
            _linkedAccountService.Setup(x => x.GetLinkedAccounts(_userSession.GpUserSession))
                .ReturnsAsync(linkedAccountResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            var subject = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            subject.StatusCode.Value.Should().Equals(HttpStatusCode.OK);
            subject.Value.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetAccessSummaryOfLinkedAccount_ReturnsSuccessfulResult_WhenServicesReturnsSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var odsCode = _fixture.Create<string>();
            var organisationName = _fixture.Create<string>();
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

            var linkedAccountAccessSummaryResponse = _fixture.Create<GetLinkedAccountAccessSummaryResponse>();
            LinkedAccountAccessSummaryResult linkedAccountSummaryResult = new LinkedAccountAccessSummaryResult.Success(linkedAccountAccessSummaryResponse);

            _linkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_userSession.GpUserSession, id))
                .Returns(odsCode)
                .Verifiable();

            _linkedAccountService.Setup(x => x.GetLinkedAccount(_userSession.GpUserSession, id))
                .ReturnsAsync(linkedAccountSummaryResult)
                .Verifiable();

            _gpSearchService.Setup(x => x.GetGpPracticeByOdsCode(odsCode))
                .ReturnsAsync(gpSearchResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAccessSummaryOfLinkedAccount(id);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
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
            var odsCode = _fixture.Create<string>();
            var gpSearchResponse = new GpSearchResponse();
            var gpSearchResult = new GpSearchResult.Success(gpSearchResponse);

            var linkedAccountAccessSummaryResponse = _fixture.Create<GetLinkedAccountAccessSummaryResponse>();
            LinkedAccountAccessSummaryResult linkedAccountSummaryResult = new LinkedAccountAccessSummaryResult.Success(linkedAccountAccessSummaryResponse);

            _linkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_userSession.GpUserSession, id))
                .Returns(odsCode)
                .Verifiable();

            _linkedAccountService.Setup(x => x.GetLinkedAccount(_userSession.GpUserSession, id))
                .ReturnsAsync(linkedAccountSummaryResult)
                .Verifiable();

            _gpSearchService.Setup(x => x.GetGpPracticeByOdsCode(odsCode))
                .ReturnsAsync(gpSearchResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAccessSummaryOfLinkedAccount(id);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
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
            var odsCode = _fixture.Create<string>();
            var gpSearchResult = new GpSearchResult.InternalServerError();

            var linkedAccountAccessSummaryResponse = _fixture.Create<GetLinkedAccountAccessSummaryResponse>();
            LinkedAccountAccessSummaryResult linkedAccountSummaryResult = new LinkedAccountAccessSummaryResult.Success(linkedAccountAccessSummaryResponse);

            _linkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_userSession.GpUserSession, id))
                .Returns(odsCode)
                .Verifiable();

            _linkedAccountService.Setup(x => x.GetLinkedAccount(_userSession.GpUserSession, id))
                .ReturnsAsync(linkedAccountSummaryResult)
                .Verifiable();

            _gpSearchService.Setup(x => x.GetGpPracticeByOdsCode(odsCode))
                .ReturnsAsync(gpSearchResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAccessSummaryOfLinkedAccount(id);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
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
            var odsCode = _fixture.Create<string>();
            var gpSearchResponse = new GpSearchResponse();
            var gpSearchResult = new GpSearchResult.Success(gpSearchResponse);

            LinkedAccountAccessSummaryResult linkedAccountSummaryResult = new LinkedAccountAccessSummaryResult.BadGateway();

            _linkedAccountService.Setup(x => x.GetOdsCodeForLinkedAccount(_userSession.GpUserSession, id))
                .Returns(odsCode)
                .Verifiable();

            _linkedAccountService.Setup(x => x.GetLinkedAccount(_userSession.GpUserSession, id))
                .ReturnsAsync(linkedAccountSummaryResult)
                .Verifiable();

            _gpSearchService.Setup(x => x.GetGpPracticeByOdsCode(odsCode))
                .ReturnsAsync(gpSearchResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAccessSummaryOfLinkedAccount(id);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            _linkedAccountService.Verify();
            _gpSearchService.Verify();
            var subject = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }
    }
}
