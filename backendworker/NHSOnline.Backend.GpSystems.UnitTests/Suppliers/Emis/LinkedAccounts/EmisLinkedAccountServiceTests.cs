using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.LinkedAccounts
{
    [TestClass]
    public class EmisLinkedAccountServiceTests
    {
        private EmisLinkedAccountsService _systemUnderTest;
        private Mock<IEmisDemographicsService> _demographicsService;
        private Mock<IEmisClient> _emisClient;
        private EmisConfigurationSettings _settings;
        private EmisUserSession _emisUserSession;
        private IFixture _fixture;
        private ILogger<EmisLinkedAccountsService> _logger;
        private const string DefaultEmisVersion = "2.1.0.0";
        private static readonly string DefaultEmisApplicationId = Guid.NewGuid().ToString();
        private static readonly Uri BaseUri = new Uri("http://emis_base_url/");
        private const string CertificatePath = "CertificatePath";
        private const string CertificatePassphrase = "CerticiatePassphrase";
        private const int CoursesMaxCoursesLimit = 100;
        private const int EmisExtendedHttpTimeoutSeconds = 6;
        private const int DefaultHttpTimeoutSeconds = 2;
        private const int PrescriptionsMaxCoursesSoftLimit = 100;
        private const string Environment = "environment";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisUserSession = _fixture.Create<EmisUserSession>();

            _logger = Mock.Of<ILogger<EmisLinkedAccountsService>>();

            _demographicsService = new Mock<IEmisDemographicsService>();
            _emisClient = new Mock<IEmisClient>();

            _settings = new EmisConfigurationSettings(BaseUri, DefaultEmisApplicationId, DefaultEmisVersion, CertificatePath,
                CertificatePassphrase, EmisExtendedHttpTimeoutSeconds, DefaultHttpTimeoutSeconds, CoursesMaxCoursesLimit, PrescriptionsMaxCoursesSoftLimit,
                Environment);
            _fixture.Inject(_settings);
            _fixture.Inject(_logger);
            _fixture.Inject(_demographicsService);
            _fixture.Inject(_emisClient);
            _systemUnderTest = _fixture.Create<EmisLinkedAccountsService>();
        }

        [TestMethod]
        public void GetOdsCodeForLinkedAccount_ReturnsOdsCode_WhenLinkedAccountWithMatchingIdFoundInUserSession()
        {
            // Arrange
            var proxyId = Guid.NewGuid();
            var proxyOdsCode = _fixture.Create<string>();

            _emisUserSession = new EmisUserSession
            {
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        OdsCode = _fixture.Create<string>(),
                    },
                    new EmisProxyUserSession
                    {
                        Id = proxyId,
                        OdsCode = proxyOdsCode,
                    },
                }
            };

            // Act
            var resultOdsCode = _systemUnderTest.GetOdsCodeForLinkedAccount(_emisUserSession, proxyId);

            // Assert
            resultOdsCode.Should().Be(proxyOdsCode);
        }

        [TestMethod]
        public void GetOdsCodeForLinkedAccount_ReturnsNull_WhenLinkedAccountWithMatchingIdFoundInUserSession()
        {
            // Arrange
            var randomGuidWhichWontBeFound = Guid.NewGuid();

            _emisUserSession = new EmisUserSession
            {
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        OdsCode = _fixture.Create<string>(),
                    },
                    new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        OdsCode = _fixture.Create<string>(),
                    },
                }
            };

            // Act
            var resultOdsCode = _systemUnderTest.GetOdsCodeForLinkedAccount(_emisUserSession, randomGuidWhichWontBeFound);

            // Assert
            resultOdsCode.Should().BeNull();
        }

        [TestMethod]
        public async Task GetLinkedAccount_ReturnsNotFoundResult_WhenProxyUserNotFoundInSession()
        {
            // Act
            var result = await _systemUnderTest.GetLinkedAccount(_emisUserSession, Guid.NewGuid());

            // Assert
            result.Should().BeOfType<LinkedAccountAccessSummaryResult.NotFound>();
        }

        [TestMethod]
        public async Task GetLinkedAccount_ReturnsSuccessResult_WhenSettingsReturnedAreSuccessful()
        {
            // Arrange
            var proxyAccountToUse = _emisUserSession.ProxyPatients.First();

            var settingsResponse = _fixture.Create<MeSettingsGetResponse>();
            var settingsResult = new EmisClient.EmisApiObjectResponse<MeSettingsGetResponse>(HttpStatusCode.Accepted)
            {
                Body = settingsResponse,
            };

            _emisClient.Setup(x => x.MeSettingsGet(proxyAccountToUse.UserPatientLinkToken,
                It.Is<EmisHeaderParameters>(e =>
                    string.Equals(e.SessionId, _emisUserSession.SessionId, StringComparison.Ordinal) &&
                    string.Equals(e.EndUserSessionId, _emisUserSession.EndUserSessionId, StringComparison.Ordinal))))
                .ReturnsAsync(settingsResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetLinkedAccount(_emisUserSession, proxyAccountToUse.Id);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountAccessSummaryResult.Success>().Subject;
            successResult.Response.CanBookAppointment.Should().Be(settingsResponse.AssignedServices.AppointmentsEnabled);
            successResult.Response.CanOrderRepeatPrescription.Should().Be(settingsResponse.AssignedServices.PrescribingEnabled);
            successResult.Response.CanViewMedicalRecord.Should().Be(settingsResponse.AssignedServices.MedicalRecordEnabled);
            _emisClient.Verify();
        }

        [TestMethod]
        public async Task GetLinkedAccount_ReturnsBadGatewayResult_WhenSettingsFails()
        {
            // Arrange
            var proxyAccountToUse = _emisUserSession.ProxyPatients.First();

            var settingsResult =
                new EmisClient.EmisApiObjectResponse<MeSettingsGetResponse>(HttpStatusCode.InternalServerError);

            _emisClient.Setup(x => x.MeSettingsGet(proxyAccountToUse.UserPatientLinkToken,
                It.Is<EmisHeaderParameters>(e =>
                    string.Equals(e.SessionId, _emisUserSession.SessionId, StringComparison.Ordinal) &&
                    string.Equals(e.EndUserSessionId, _emisUserSession.EndUserSessionId, StringComparison.Ordinal))))
                .ReturnsAsync(settingsResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetLinkedAccount(_emisUserSession, proxyAccountToUse.Id);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountAccessSummaryResult.BadGateway>();
            _demographicsService.Verify();
            _emisClient.Verify();
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsSuccessResponse_WhenSuccessfulResponseFromEmis()
        {
            _emisUserSession.HasLinkedAccounts = true;
            _emisUserSession.ProxyPatients = _fixture.Create<List<EmisProxyUserSession>>();

            var demographicsResponses = new Dictionary<Guid, DemographicsResponse>();

            foreach (var user in _emisUserSession.ProxyPatients)
            {
                var demographicsResponse = _fixture.Create<DemographicsResponse>();
                demographicsResponses.Add(user.Id, demographicsResponse);
                DemographicsResult demographicsResult = new DemographicsResult.Success(demographicsResponse);

                _demographicsService
                    .Setup(x => x.GetDemographics(
                        It.Is<GpLinkedAccountModel>(e => string.Equals( ((EmisUserSession) e.GpUserSession).SessionId, _emisUserSession.SessionId, StringComparison.Ordinal)
                        && string.Equals(((EmisUserSession) e.GpUserSession).EndUserSessionId, _emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                        && string.Equals(((EmisUserSession) e.GpUserSession).UserPatientLinkToken, user.UserPatientLinkToken, StringComparison.Ordinal))))
                    .Returns(Task.FromResult(demographicsResult))
                    .Verifiable();
            }

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.Response.Should().NotBeNull();
            successResult.Response.LinkedAccounts.Count().Should().Be(_emisUserSession.ProxyPatients.Count);

            for (var i = 0; i < _emisUserSession.ProxyPatients.Count; i++)
            {
                var userDetail = _emisUserSession.ProxyPatients.ElementAt(i);
                var demographicsResponseForUser = demographicsResponses[userDetail.Id];
                var linkedAccountDetail = successResult.Response.LinkedAccounts.ElementAt(i);

                linkedAccountDetail.Id.Should().Be(userDetail.Id);
                linkedAccountDetail.NhsNumber.Should().Be(demographicsResponseForUser.NhsNumber);
                linkedAccountDetail.Name.Should().Be(demographicsResponseForUser.PatientName);
                linkedAccountDetail.DateOfBirth.Should().Be(demographicsResponseForUser.DateOfBirth);
            }

            _demographicsService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsEmptyResponse_WhenHasLinkedAccountsIsFalse()
        {
            _emisUserSession.HasLinkedAccounts = false;
            _emisUserSession.ProxyPatients = _fixture.Create<List<EmisProxyUserSession>>();

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.Response.Should().NotBeNull();
            successResult.Response.LinkedAccounts.Count().Should().Be(0);
            _demographicsService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsEmptyResponse_WhenDemographicServiceReturnsNothing()
        {
            _emisUserSession.HasLinkedAccounts = true;
            _emisUserSession.ProxyPatients = new List<EmisProxyUserSession>();

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.Response.Should().NotBeNull();
            successResult.Response.LinkedAccounts.Count().Should().Be(0);
            _demographicsService.VerifyAll();
        }
    }
}