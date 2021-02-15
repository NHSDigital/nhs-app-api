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
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.LinkedAccounts
{
    [TestClass]
    public class EmisLinkedAccountServiceTests
    {
        private EmisLinkedAccountsService _systemUnderTest;
        private Mock<IEmisDemographicsService> _demographicsService;
        private Mock<IEmisClient> _emisClient;
        private Mock<ICurrentDateTimeProvider> _timeProvider;
        private EmisConfigurationSettings _settings;
        private EmisUserSession _emisUserSession;
        private IFixture _fixture;
        private Mock<ILogger<EmisLinkedAccountsService>> _logger;
        private const string DefaultEmisVersion = "2.1.0.0";
        private static readonly string DefaultEmisApplicationId = Guid.NewGuid().ToString();
        private static readonly Uri BaseUri = new Uri("http://emis_base_url/");
        private const string CertificatePath = "CertificatePath";
        private const string CertificatePassphrase = "CerticiatePassphrase";
        private const int CoursesMaxCoursesLimit = 100;
        private const int EmisExtendedHttpTimeoutSeconds = 6;
        private const int DefaultHttpTimeoutSeconds = 2;
        private const int PrescriptionsMaxCoursesSoftLimit = 100;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisUserSession = _fixture.Create<EmisUserSession>();

            _logger = _fixture.Freeze<Mock<ILogger<EmisLinkedAccountsService>>>();

            _demographicsService = new Mock<IEmisDemographicsService>();
            _emisClient = new Mock<IEmisClient>();

            _timeProvider = new Mock<ICurrentDateTimeProvider>();

            _settings = new EmisConfigurationSettings(BaseUri, DefaultEmisApplicationId, DefaultEmisVersion, CertificatePath,
                CertificatePassphrase, EmisExtendedHttpTimeoutSeconds, DefaultHttpTimeoutSeconds, CoursesMaxCoursesLimit, PrescriptionsMaxCoursesSoftLimit);

            _fixture.Inject(_settings);
            _fixture.Inject(_demographicsService);
            _fixture.Inject(_emisClient);
            _fixture.Inject(_timeProvider);

            _systemUnderTest = _fixture.Create<EmisLinkedAccountsService>();
            _sampleSuccessStatusCodes = new List<HttpStatusCode>()
            {
                HttpStatusCode.OK
            };
        }

        [TestMethod]
        public void GetOdsCodeForLinkedAccount_ReturnsOdsCode_WhenLinkedAccountWithMatchingIdFoundInUserSession()
        {
            // Arrange
            var proxyPatientGpIdentifier = _fixture.Create<string>();
            var proxyOdsCode = _fixture.Create<string>();

            _emisUserSession = new EmisUserSession
            {
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>(),
                        OdsCode = _fixture.Create<string>(),
                    },
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = proxyPatientGpIdentifier,
                        OdsCode = proxyOdsCode,
                    },
                }
            };

            // Act
            var resultOdsCode = _systemUnderTest.GetOdsCodeForLinkedAccount(_emisUserSession, proxyPatientGpIdentifier);

            // Assert
            resultOdsCode.Should().Be(proxyOdsCode);
        }

        [TestMethod]
        public void GetOdsCodeForLinkedAccount_ReturnsNotFoundResult_WhenIdNotFoundInUserSession()
        {
            // Arrange
            var randomIdWhichWontBeFound = _fixture.Create<string>();

            _emisUserSession = new EmisUserSession
            {
                PatientActivityContextGuid = _fixture.Create<string>(),
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>(),
                        OdsCode = _fixture.Create<string>(),
                    },
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>(),
                        OdsCode = _fixture.Create<string>(),
                    },
                }
            };

            // Act
            var resultOdsCode = _systemUnderTest.GetOdsCodeForLinkedAccount(_emisUserSession, randomIdWhichWontBeFound);

            // Assert
            resultOdsCode.Should().BeNull();
        }

        [TestMethod]
        public async Task SwitchAccount_ReturnsSuccess_WhenLinkedAccountWithMatchingIdFoundInUserSessionForProxy()
        {
            // Arrange
            var proxyPatientPatientIdentifier = _fixture.Create<string>();

            _emisUserSession = new EmisUserSession
            {
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = proxyPatientPatientIdentifier,
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            // Act
            var result = await _systemUnderTest.SwitchAccount(_emisUserSession, proxyPatientPatientIdentifier);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.Success>();
        }

        [TestMethod]
        public async Task SwitchAccount_ReturnsSuccessAndUpdatesPatientNhsNumber_WhenLinkedAccountWithMatchingIdFoundInUserSessionWithNoNhsNumber()
        {
            // Arrange
            var proxyPatientPatientIdentifier = _fixture.Create<string>();
            var proxyUserPatientLinkToken = _fixture.Create<string>();

            var emisProxyWithNoNhsNumber = new EmisProxyUserSession
            {
                PatientActivityContextGuid = proxyPatientPatientIdentifier,
                UserPatientLinkToken = proxyUserPatientLinkToken,
                NhsNumber = null
            };

            _emisUserSession = new EmisUserSession
            {
                SessionId = _fixture.Create<string>(),
                EndUserSessionId = _fixture.Create<string>(),
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>(),
                    },
                    emisProxyWithNoNhsNumber
                }
            };

            var demographicsResponse = _fixture.Create<DemographicsResponse>();
            var demographicsResult = new DemographicsResult.Success(demographicsResponse);

            _demographicsService
                .Setup(x => x.GetDemographics(
                    It.Is<GpLinkedAccountModel>(e => string.Equals( ((EmisUserSession) e.GpUserSession).SessionId, _emisUserSession.SessionId, StringComparison.Ordinal)
                                                     && string.Equals(((EmisUserSession) e.GpUserSession).EndUserSessionId, _emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                                                     && string.Equals(((EmisUserSession) e.GpUserSession).UserPatientLinkToken, proxyUserPatientLinkToken, StringComparison.Ordinal)
                                                     && string.Equals(((EmisUserSession) e.GpUserSession).PatientActivityContextGuid, proxyPatientPatientIdentifier, StringComparison.Ordinal)
                                                     && e.RequestingPatientGpIdentifier == proxyPatientPatientIdentifier)))
                .ReturnsAsync(demographicsResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.SwitchAccount(_emisUserSession, proxyPatientPatientIdentifier);

            // Assert
            _demographicsService.Verify();
            result.Should().BeOfType<SwitchAccountResult.Success>();
            emisProxyWithNoNhsNumber.NhsNumber.Should().Be(demographicsResponse.NhsNumber);
        }

        [TestMethod]
        public async Task SwitchAccount_ReturnsTrue_WhenMainAccountWithMatchingIdFoundInUserSession()
        {
            // Arrange
            var mainUserPatientIdentifier = _fixture.Create<string>();

            _emisUserSession = new EmisUserSession
            {
                PatientActivityContextGuid = mainUserPatientIdentifier,
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>()
                    },
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>()
                    },
                }
            };

            // Act
            var result = await _systemUnderTest.SwitchAccount(_emisUserSession, mainUserPatientIdentifier);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.Success>();
        }

        [TestMethod]
        public async Task SwitchAccount_ReturnsNotFoundResult_WhenLinkedAccountWithMatchingIdIsNotFoundInUserSession()
        {
            // Arrange
            var randomPatientIdentifierWhichWontBeFound = _fixture.Create<string>();

            _emisUserSession = new EmisUserSession
            {
                PatientActivityContextGuid = _fixture.Create<string>(),
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>(),
                    },
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>(),
                    },
                }
            };

            // Act
            var result = await _systemUnderTest.SwitchAccount(_emisUserSession, randomPatientIdentifierWhichWontBeFound);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.NotFound>();
        }

        [TestMethod]
        public async Task GetLinkedAccount_ReturnsNotFoundResult_WhenProxyUserNotFoundInSession()
        {
            // Act
            var result = await _systemUnderTest.GetLinkedAccount(_emisUserSession, _fixture.Create<string>());

            // Assert
            result.Should().BeOfType<LinkedAccountAccessSummaryResult.NotFound>();
        }

        [TestMethod]
        public async Task GetLinkedAccount_ReturnsSuccessResult_WhenSettingsReturnedAreSuccessful()
        {
            // Arrange
            var proxyAccountToUse = _emisUserSession.ProxyPatients.First();

            var settingsResponse = _fixture.Create<MeSettingsGetResponse>();
            var settingsResult = new EmisApiObjectResponse<MeSettingsGetResponse>(HttpStatusCode.Accepted, RequestsForSuccessOutcome.MeSettingsGet, _sampleSuccessStatusCodes)
            {
                Body = settingsResponse,
            };

            _emisClient.Setup(x => x.MeSettingsGet(
                It.Is<EmisRequestParameters>(e =>
                    string.Equals(e.SessionId, _emisUserSession.SessionId, StringComparison.Ordinal) &&
                    string.Equals(e.EndUserSessionId, _emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                    string.Equals(e.UserPatientLinkToken, proxyAccountToUse.UserPatientLinkToken, StringComparison.Ordinal))))
                .ReturnsAsync(settingsResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetLinkedAccount(_emisUserSession, proxyAccountToUse.PatientActivityContextGuid);

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
                new EmisApiObjectResponse<MeSettingsGetResponse>(HttpStatusCode.InternalServerError, RequestsForSuccessOutcome.MeSettingsGet, _sampleSuccessStatusCodes);

            _emisClient.Setup(x => x.MeSettingsGet(
                It.Is<EmisRequestParameters>(e =>
                    string.Equals(e.SessionId, _emisUserSession.SessionId, StringComparison.Ordinal) &&
                    string.Equals(e.EndUserSessionId, _emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                    string.Equals(e.UserPatientLinkToken, proxyAccountToUse.UserPatientLinkToken, StringComparison.Ordinal))))
                .ReturnsAsync(settingsResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetLinkedAccount(_emisUserSession, proxyAccountToUse.PatientActivityContextGuid);

            // Assert
            result.Should().BeOfType<LinkedAccountAccessSummaryResult.BadGateway>();
            _demographicsService.Verify();
            _emisClient.Verify();
        }

      [TestMethod]
        public async Task GetLinkedAccounts_ReturnsSuccessResponse_WhenSuccessfulResponseFromEmis()
        {
            var proxyPatients = _fixture.Create<List<EmisProxyUserSession>>();
            proxyPatients.ForEach(x => x.OdsCode = _emisUserSession.OdsCode); // set all to have same Ods code
            proxyPatients.ForEach(x => x.NhsNumber = null); // set all to have null NhsNumber (simulate first time)
            _emisUserSession.ProxyPatients = proxyPatients;

            var sessionIdGuidToPatientGpIdentifier = new Dictionary<Guid, string>();
            var demographicsResponses = new Dictionary<string, DemographicsResponse>();

            foreach (var user in _emisUserSession.ProxyPatients)
            {
                sessionIdGuidToPatientGpIdentifier.Add(Guid.NewGuid(), user.PatientActivityContextGuid);
                var demographicsResponse = _fixture.Create<DemographicsResponse>();

                demographicsResponses.Add(user.PatientActivityContextGuid, demographicsResponse);
                demographicsResponse.DateOfBirth = new DateTime(1972, 04, 12, 0, 0, 0, DateTimeKind.Utc);
                var demographicsResult = new DemographicsResult.Success(demographicsResponse);

                _demographicsService
                    .Setup(x => x.GetDemographics(
                        It.Is<GpLinkedAccountModel>(e => string.Equals( ((EmisUserSession) e.GpUserSession).SessionId, _emisUserSession.SessionId, StringComparison.Ordinal)
                                                         && string.Equals(((EmisUserSession) e.GpUserSession).EndUserSessionId, _emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                                                         && string.Equals(((EmisUserSession) e.GpUserSession).UserPatientLinkToken, user.UserPatientLinkToken, StringComparison.Ordinal)
                                                         && string.Equals(((EmisUserSession) e.GpUserSession).PatientActivityContextGuid, user.PatientActivityContextGuid, StringComparison.Ordinal)
                                                         && e.RequestingPatientGpIdentifier == user.PatientActivityContextGuid)))
                    .ReturnsAsync(demographicsResult)
                    .Verifiable();
            }

            _timeProvider.Setup(x => x.LocalNow).Returns(new DateTime(2021, 06, 12, 4, 32, 0, DateTimeKind.Utc));

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession, sessionIdGuidToPatientGpIdentifier);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.Should().NotBeNull();
            successResult.ValidAccounts.Count().Should().Be(_emisUserSession.ProxyPatients.Count);
            successResult.HasAnyProxyInfoBeenUpdatedInSession.Should().BeTrue();

            for (var i = 0; i < _emisUserSession.ProxyPatients.Count; i++)
            {
                var emisProxyPatient = _emisUserSession.ProxyPatients.ElementAt(i);
                var demographicsResponseForUser = demographicsResponses[emisProxyPatient.PatientActivityContextGuid];
                var linkedAccountDetail = successResult.ValidAccounts.ElementAt(i);

                emisProxyPatient.NhsNumber.Should().Be(demographicsResponseForUser.NhsNumber);

                var sessionId = sessionIdGuidToPatientGpIdentifier
                    .First(x => x.Value == emisProxyPatient.PatientActivityContextGuid).Key;

                linkedAccountDetail.Id.Should().Be(sessionId);
                linkedAccountDetail.FullName.Should().Be(demographicsResponseForUser.PatientName);
                linkedAccountDetail.AgeMonths.Should().Be(0);
                linkedAccountDetail.AgeYears.Should().Be(49);
            }

            _demographicsService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsFalseForNeedsSessionUpdate_WhenNhsNumbersAlreadySetOnSession()
        {
            var proxyPatients = _fixture.Create<List<EmisProxyUserSession>>(); // will auto set all NHS numbers
            proxyPatients.ForEach(x => x.OdsCode = _emisUserSession.OdsCode); // set all to have same Ods code

            _emisUserSession.ProxyPatients = proxyPatients;

            var sessionIdGuidToPatientGpIdentifier = new Dictionary<Guid, string>();
            var demographicsResponses = new Dictionary<string, DemographicsResponse>();

            foreach (var user in _emisUserSession.ProxyPatients)
            {
                sessionIdGuidToPatientGpIdentifier.Add(Guid.NewGuid(), user.PatientActivityContextGuid);
                var demographicsResponse = _fixture.Create<DemographicsResponse>();
                demographicsResponse.NhsNumber = user.NhsNumber; // make sure it's not different
                demographicsResponses.Add(user.PatientActivityContextGuid, demographicsResponse);
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
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession, sessionIdGuidToPatientGpIdentifier);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.Should().NotBeNull();
            successResult.HasAnyProxyInfoBeenUpdatedInSession.Should().BeFalse();

            _demographicsService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsSuccessResponse_CorrectlyFiltersLinkedAccounts()
        {
            _emisUserSession.ProxyPatients = new List<EmisProxyUserSession>();

            var patientSetup = new []
            {
                new
                {
                    // non-matching ODS code
                    Id = Guid.NewGuid(), PatientActivityContextGuid = "101", UserPatientLinkToken = "501", NhsNumber = "9000000001", OdsCode = "123"
                },
                new
                {
                    Id = Guid.NewGuid(), PatientActivityContextGuid = "102", UserPatientLinkToken = "502", NhsNumber = "9000000002", OdsCode = _emisUserSession.OdsCode,
                },
                new
                {
                    // null NHS number
                    Id = Guid.NewGuid(), PatientActivityContextGuid = "103", UserPatientLinkToken = "503", NhsNumber = (string)null, OdsCode = _emisUserSession.OdsCode,
                },
            };

            foreach (var patient in patientSetup)
            {
                _emisUserSession.ProxyPatients.Add(new EmisProxyUserSession
                {
                    OdsCode = patient.OdsCode,
                    UserPatientLinkToken = patient.UserPatientLinkToken,
                    PatientActivityContextGuid = patient.PatientActivityContextGuid,
                });
            }

            var sessionIdGuidToPatientGpIdentifier = new Dictionary<Guid, string>();
            var demographicsResponses = new Dictionary<Guid, DemographicsResponse>();

            foreach (var patient in patientSetup)
            {
                sessionIdGuidToPatientGpIdentifier.Add(patient.Id, patient.PatientActivityContextGuid);

                var demographicsResponse = _fixture.Create<DemographicsResponse>();
                demographicsResponse.NhsNumber = patient.NhsNumber;
                demographicsResponses.Add(patient.Id, demographicsResponse);
                DemographicsResult demographicsResult = new DemographicsResult.Success(demographicsResponse);

                _demographicsService
                    .Setup(x => x.GetDemographics(
                        It.Is<GpLinkedAccountModel>(e => string.Equals( ((EmisUserSession) e.GpUserSession).SessionId, _emisUserSession.SessionId, StringComparison.Ordinal)
                        && string.Equals(((EmisUserSession) e.GpUserSession).EndUserSessionId, _emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                        && string.Equals(((EmisUserSession) e.GpUserSession).UserPatientLinkToken, patient.UserPatientLinkToken, StringComparison.Ordinal))))
                    .Returns(Task.FromResult(demographicsResult))
                    .Verifiable();
            }

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession, sessionIdGuidToPatientGpIdentifier);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.Should().NotBeNull();
            successResult.ValidAccounts.Count().Should().Be(2);

            var expectedLogMessage =
                $"Linked_profiles_count=3, " +
                $"excluded_for_not_having_NHS_number=1, " +
                $"has_different_ODS_code=1, " +
                $"valid_and_being_returned: 2";
            _logger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());

            foreach (var returnedValidLinkedAccount in successResult.ValidAccounts)
            {
                var patientGpIdentifier = sessionIdGuidToPatientGpIdentifier[returnedValidLinkedAccount.Id];
                var emisProxyPatient = _emisUserSession.ProxyPatients.
                    FirstOrDefault(x => x.PatientActivityContextGuid == patientGpIdentifier);

                var demographicDataForValidUser = demographicsResponses[returnedValidLinkedAccount.Id];

                returnedValidLinkedAccount.FullName.Should().Be(demographicDataForValidUser.PatientName);
                returnedValidLinkedAccount.GivenName.Should().Be(demographicDataForValidUser.NameParts.Given);
                emisProxyPatient.NhsNumber.Should().Be(demographicDataForValidUser.NhsNumber);
            }
            _demographicsService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsEmptyResponse_WhenHasLinkedAccountsIsFalse()
        {
            _emisUserSession.ProxyPatients = new List<EmisProxyUserSession>();

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession, new Dictionary<Guid, string>());

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.Should().NotBeNull();
            successResult.ValidAccounts.Count().Should().Be(0);
            _demographicsService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsEmptyResponse_WhenDemographicServiceReturnsNothing()
        {
            _emisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession
                {
                    UserPatientLinkToken = _fixture.Create<string>(),
                    PatientActivityContextGuid = _fixture.Create<string>(),
                }
            };

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession, new Dictionary<Guid, string>());

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.Should().NotBeNull();
            successResult.ValidAccounts.Count().Should().Be(0);
            _demographicsService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccounts_HandlesCaseWhereNotAllDemographicCallsAreSuccessful()
        {
            var proxyPatients = _fixture.Create<List<EmisProxyUserSession>>();
            proxyPatients.ForEach(x => x.OdsCode = _emisUserSession.OdsCode); // set all to have same Ods code
            proxyPatients.ForEach(x => x.NhsNumber = null); // set all to have null NhsNumber (simulate first time)
            _emisUserSession.ProxyPatients = proxyPatients;
            var demographicsResponses = new Dictionary<string, DemographicsResponse>();

            var sessionIdGuidToPatientGpIdentifier = new Dictionary<Guid, string>();
            var patientToFailDemographicsFor = proxyPatients.First();

            foreach (var user in _emisUserSession.ProxyPatients)
            {
                sessionIdGuidToPatientGpIdentifier.Add(Guid.NewGuid(), user.PatientActivityContextGuid);

                var demographicsResponse = _fixture.Create<DemographicsResponse>();
                demographicsResponses.Add(user.PatientActivityContextGuid, demographicsResponse);
                demographicsResponse.DateOfBirth = new DateTime(1972, 04, 12, 0, 0, 0, DateTimeKind.Utc);
                DemographicsResult demographicsResult = new DemographicsResult.Success(demographicsResponse);

                if (patientToFailDemographicsFor == user)
                {
                    demographicsResult = new DemographicsResult.Forbidden();
                }

                _demographicsService
                    .Setup(x => x.GetDemographics(
                        It.Is<GpLinkedAccountModel>(e =>
                            string.Equals(((EmisUserSession) e.GpUserSession).SessionId, _emisUserSession.SessionId,
                                StringComparison.Ordinal)
                            && string.Equals(((EmisUserSession) e.GpUserSession).EndUserSessionId,
                                _emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                            && string.Equals(((EmisUserSession) e.GpUserSession).UserPatientLinkToken,
                                user.UserPatientLinkToken, StringComparison.Ordinal))))
                    .Returns(Task.FromResult(demographicsResult))
                    .Verifiable();
            }

            _timeProvider.Setup(x => x.LocalNow).Returns(new DateTime(2021, 06, 12, 4, 32, 0, DateTimeKind.Utc));

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession, sessionIdGuidToPatientGpIdentifier);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;

            successResult.Should().NotBeNull();
            // valid accounts count should be one less as we returned a non-success for one demographics call
            successResult.ValidAccounts.Count().Should().Be(_emisUserSession.ProxyPatients.Count - 1);
            successResult.HasAnyProxyInfoBeenUpdatedInSession.Should().BeTrue();

            var validProxyPatients = _emisUserSession.ProxyPatients.Except(new[] { patientToFailDemographicsFor });
            for (var i = 0; i < validProxyPatients.Count(); i++)
            {
                var emisProxyPatient = validProxyPatients.ElementAt(i);
                var demographicsResponseForUser = demographicsResponses[emisProxyPatient.PatientActivityContextGuid];
                var linkedAccountDetail = successResult.ValidAccounts.ElementAt(i);
                var sessionId =
                    sessionIdGuidToPatientGpIdentifier.First(x => x.Value == emisProxyPatient.PatientActivityContextGuid).Key;

                emisProxyPatient.NhsNumber.Should().Be(demographicsResponseForUser.NhsNumber);

                linkedAccountDetail.Id.Should().Be(sessionId);
                linkedAccountDetail.FullName.Should().Be(demographicsResponseForUser.PatientName);
                linkedAccountDetail.AgeMonths.Should().Be(0);
                linkedAccountDetail.AgeYears.Should().Be(49);
            }

            _demographicsService.VerifyAll();
            _logger.VerifyLogger(LogLevel.Warning, "Not all demographics calls for proxy patients were successful.", Times.Once());
        }

        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdFoundInProxyUser()
        {
            // Arrange
            var patientGpIdentifier = _fixture.Create<string>();
            var proxyNhsNumber = _fixture.Create<string>();

            _emisUserSession = new EmisUserSession
            {
                PatientActivityContextGuid = _fixture.Create<string>(),
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = patientGpIdentifier,
                        NhsNumber = proxyNhsNumber
                    },
                }
            };

            // Act
            var result = _systemUnderTest.GetProxyAuditData(
                new GpLinkedAccountModel(_emisUserSession, patientGpIdentifier));

            // Assert
            result.IsProxyMode.Should().Be(true);
            result.ProxyNhsNumber.Should().Be(proxyNhsNumber);
        }


        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdFoundInMainUser()
        {
            // Arrange
            var patientGpIdentifier = _fixture.Create<string>();

            _emisUserSession = new EmisUserSession
            {
                PatientActivityContextGuid = patientGpIdentifier,
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new EmisProxyUserSession
                    {
                        PatientActivityContextGuid = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            // Act
            var result = _systemUnderTest.GetProxyAuditData(
                new GpLinkedAccountModel(_emisUserSession, patientGpIdentifier));

            // Assert
            result.IsProxyMode.Should().Be(false);
            result.ProxyNhsNumber.Should().Be(null);
        }

        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdNotFound()
        {
            //Arrange
            var patientGpIdentifier = _fixture.Create<string>();

            _emisUserSession = new EmisUserSession
            {
                UserPatientLinkToken = _fixture.Create<string>(),
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        UserPatientLinkToken = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new EmisProxyUserSession
                    {
                        UserPatientLinkToken = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            //Act
            var result = _systemUnderTest.GetProxyAuditData(
                new GpLinkedAccountModel(_emisUserSession, patientGpIdentifier));

            //Assert
            result.IsProxyMode.Should().Be(false);
            result.ProxyNhsNumber.Should().Be(null);
        }
    }
}