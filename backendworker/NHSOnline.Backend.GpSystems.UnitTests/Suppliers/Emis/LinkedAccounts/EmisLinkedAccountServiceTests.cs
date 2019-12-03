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
using UnitTestHelper;

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
        private const string Environment = "environment";
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisUserSession = _fixture.Create<EmisUserSession>();

            _logger = _fixture.Freeze<Mock<ILogger<EmisLinkedAccountsService>>>();

            _demographicsService = new Mock<IEmisDemographicsService>();
            _emisClient = new Mock<IEmisClient>();

            _settings = new EmisConfigurationSettings(BaseUri, DefaultEmisApplicationId, DefaultEmisVersion, CertificatePath,
                CertificatePassphrase, EmisExtendedHttpTimeoutSeconds, DefaultHttpTimeoutSeconds, CoursesMaxCoursesLimit, PrescriptionsMaxCoursesSoftLimit,
                Environment);
            _fixture.Inject(_settings);
            _fixture.Inject(_demographicsService);
            _fixture.Inject(_emisClient);
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
        public void CalculateAgeInMonthsAndYears_ReturnsEmptyAgeDataObjectWhenDateOfBirthIsNull()
        {
            // Arrange
            DateTime? dateOfBirth = null;
            var ageDataObject = new AgeData
            {
                AgeMonths = null,
                AgeYears = null
            };

            // Act
            var calculatedAge = _systemUnderTest.CalculateAgeInMonthsAndYears(dateOfBirth);

            // Assert
            calculatedAge.AgeMonths.Should().Be(ageDataObject.AgeMonths);
            calculatedAge.AgeYears.Should().Be(ageDataObject.AgeYears);
        }

        [TestMethod]
        public void CalculateAgeInMonthsAndYears_ReturnsCorrectAgeDataObjectWhenDateOfBirthIsValidAndGreaterThan1Year()
        {
            // Arrange
            DateTime? dateOfBirth = (DateTime.Now).AddMonths(-2);
            dateOfBirth = dateOfBirth.Value.AddYears(-5);

            var ageDataObject = new AgeData
            {
                //If the age is above 1, then the ageMonths will be 0
                AgeMonths = 0,
                AgeYears = 5
            };

            // Act
            var calculatedAge = _systemUnderTest.CalculateAgeInMonthsAndYears(dateOfBirth);

            // Assert
            calculatedAge.AgeMonths.Should().Be(ageDataObject.AgeMonths);
            calculatedAge.AgeYears.Should().Be(ageDataObject.AgeYears);
        }

        [TestMethod]
        public void CalculateAgeInMonthsAndYears_ReturnsCorrectAgeDataObjectWhenDateOfBirthIsValidAndLessThan1Year()
        {
            // Arrange
            DateTime? dateOfBirth = (DateTime.Now).AddMonths(-5);
            dateOfBirth = dateOfBirth.Value.AddYears(0);

            var ageDataObject = new AgeData
            {
                AgeMonths = 5,
                AgeYears = 0
            };

            // Act
            var calculatedAge = _systemUnderTest.CalculateAgeInMonthsAndYears(dateOfBirth);

            // Assert
            calculatedAge.AgeMonths.Should().Be(ageDataObject.AgeMonths);
            calculatedAge.AgeYears.Should().Be(ageDataObject.AgeYears);
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
        public void IsValidAccountOrLinkedAccountId_ReturnsTrue_WhenLinkedAccountWithMatchingIdFoundInUserSessionForProxy()
        {
            // Arrange
            var proxyId = Guid.NewGuid();

            _emisUserSession = new EmisUserSession
            {
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                    },
                    new EmisProxyUserSession
                    {
                        Id = proxyId,
                    },
                }
            };

            // Act
            var result = _systemUnderTest.IsValidAccountOrLinkedAccountId(_emisUserSession, proxyId);

            // Assert
            result.Should().BeTrue();
        }


        [TestMethod]
        public void IsValidAccountOrLinkedAccountId_ReturnsTrue_WhenLinkedAccountWithMatchingIdFoundInUserSessionForMainUser()
        {
            // Arrange
            var mainUserGuid = Guid.NewGuid();

            _emisUserSession = new EmisUserSession
            {
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        Id = mainUserGuid,
                    },
                    new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                    },
                }
            };

            // Act
            var result = _systemUnderTest.IsValidAccountOrLinkedAccountId(_emisUserSession, mainUserGuid);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsValidAccountOrLinkedAccountId_ReturnsFalse_WhenLinkedAccountWithMatchingIdFoundInUserSession()
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
                    },
                    new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                    },
                }
            };

            // Act
            var result = _systemUnderTest.IsValidAccountOrLinkedAccountId(_emisUserSession, randomGuidWhichWontBeFound);

            // Assert
            result.Should().BeFalse();
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
            var settingsResult = new EmisClient.EmisApiObjectResponse<MeSettingsGetResponse>(HttpStatusCode.Accepted, RequestsForSuccessOutcome.MeSettingsGet, _sampleSuccessStatusCodes)
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
                new EmisClient.EmisApiObjectResponse<MeSettingsGetResponse>(HttpStatusCode.InternalServerError, RequestsForSuccessOutcome.MeSettingsGet, _sampleSuccessStatusCodes);

            _emisClient.Setup(x => x.MeSettingsGet(
                It.Is<EmisRequestParameters>(e =>
                    string.Equals(e.SessionId, _emisUserSession.SessionId, StringComparison.Ordinal) &&
                    string.Equals(e.EndUserSessionId, _emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                    string.Equals(e.UserPatientLinkToken, proxyAccountToUse.UserPatientLinkToken, StringComparison.Ordinal))))
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
            var proxyPatients = _fixture.Create<List<EmisProxyUserSession>>();
            proxyPatients.ForEach(x => x.OdsCode = _emisUserSession.OdsCode); // set all to have same Ods code
            proxyPatients.ForEach(x => x.NhsNumber = null); // set all to have null NhsNumber (simulate first time)
            _emisUserSession.HasLinkedAccounts = true;
            _emisUserSession.ProxyPatients = proxyPatients;

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
            successResult.LinkedAccountsBreakdown.Should().NotBeNull();
            successResult.LinkedAccountsBreakdown.ValidAccounts.Count().Should().Be(_emisUserSession.ProxyPatients.Count);
            successResult.HasAnyProxyInfoBeenUpdatedInSession.Should().BeTrue();

            for (var i = 0; i < _emisUserSession.ProxyPatients.Count; i++)
            {
                var emisProxyPatient = _emisUserSession.ProxyPatients.ElementAt(i);
                var demographicsResponseForUser = demographicsResponses[emisProxyPatient.Id];
                var linkedAccountDetail = successResult.LinkedAccountsBreakdown.ValidAccounts.ElementAt(i);

                emisProxyPatient.NhsNumber.Should().Be(demographicsResponseForUser.NhsNumber);

                linkedAccountDetail.Id.Should().Be(emisProxyPatient.Id);
                linkedAccountDetail.Name.Should().Be(demographicsResponseForUser.PatientName);
                linkedAccountDetail.AgeMonths.Should().Be(_systemUnderTest.CalculateAgeInMonthsAndYears(demographicsResponseForUser.DateOfBirth).AgeMonths);
                linkedAccountDetail.AgeYears.Should().Be(_systemUnderTest.CalculateAgeInMonthsAndYears(demographicsResponseForUser.DateOfBirth).AgeYears);
            }

            _demographicsService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsFalseForNeedsSessionUpdate_WhenNhsNumbersAlreadySetOnSession()
        {
            var proxyPatients = _fixture.Create<List<EmisProxyUserSession>>(); // will auto set all NHS numbers
            proxyPatients.ForEach(x => x.OdsCode = _emisUserSession.OdsCode); // set all to have same Ods code
            _emisUserSession.HasLinkedAccounts = true;
            _emisUserSession.ProxyPatients = proxyPatients;

            var demographicsResponses = new Dictionary<Guid, DemographicsResponse>();

            foreach (var user in _emisUserSession.ProxyPatients)
            {
                var demographicsResponse = _fixture.Create<DemographicsResponse>();
                demographicsResponse.NhsNumber = user.NhsNumber; // make sure it's not different
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
            successResult.LinkedAccountsBreakdown.Should().NotBeNull();
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
                    Id = Guid.NewGuid(), UserPatientLinkToken = "501", NhsNumber = "9000000001", OdsCode = "123"
                },
                new
                {
                    Id = Guid.NewGuid(), UserPatientLinkToken = "502", NhsNumber = "9000000002", OdsCode = _emisUserSession.OdsCode,
                },
                new
                {
                    // null NHS number
                    Id = Guid.NewGuid(), UserPatientLinkToken = "503", NhsNumber = (string)null, OdsCode = _emisUserSession.OdsCode,
                },
            };

            foreach (var patient in patientSetup)
            {
                _emisUserSession.ProxyPatients.Add(new EmisProxyUserSession
                {
                    Id = patient.Id,
                    OdsCode = patient.OdsCode,
                    UserPatientLinkToken = patient.UserPatientLinkToken,
                });
            }

            _emisUserSession.HasLinkedAccounts = true;

            var demographicsResponses = new Dictionary<Guid, DemographicsResponse>();

            foreach (var patient in patientSetup)
            {
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
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.LinkedAccountsBreakdown.Should().NotBeNull();
            successResult.LinkedAccountsBreakdown.ValidAccounts.Count().Should().Be(1);
            successResult.LinkedAccountsBreakdown.AccountsWithMismatchingOdsCode.Count().Should().Be(1);
            successResult.LinkedAccountsBreakdown.AccountsWithNoNhsNumber.Count().Should().Be(1);

            var expectedLogMessage =
                $"Linked_profiles_count={3}, " +
                $"excluded_for_not_having_NHS_number={1}, " +
                $"excluding_for_having_different_ODS_code={1}, " +
                $"valid_and_being_returned: {1}";
            _logger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());

            var validEmisProxyPatient = _emisUserSession.ProxyPatients.ElementAt(1);
            var demographicsResponseForValidUser = demographicsResponses[validEmisProxyPatient.Id];
            var validLinkedAccountDetail = successResult.LinkedAccountsBreakdown.ValidAccounts.ElementAt(0);

            var noNhsNumberEmisProxyPatient = _emisUserSession.ProxyPatients.ElementAt(2);
            var demographicsResponseForNoNhsNumberUser = demographicsResponses[noNhsNumberEmisProxyPatient.Id];
            var noNhsNumberLinkedAccountDetail = successResult.LinkedAccountsBreakdown.AccountsWithNoNhsNumber.ElementAt(0);

            var notMatchingOdsCodeEmisProxyPatient = _emisUserSession.ProxyPatients.ElementAt(0);
            var demographicsResponseForNotMatchingOdsCodeUser = demographicsResponses[notMatchingOdsCodeEmisProxyPatient.Id];
            var notMatchingOdsCodeLinkedAccountDetail = successResult.LinkedAccountsBreakdown.AccountsWithMismatchingOdsCode.ElementAt(0);

            var groupedPatientData = new[]
            {
                new
                {
                    Patient = validEmisProxyPatient,
                    DemographicsResponse = demographicsResponseForValidUser,
                    LinkedAccountDetail = validLinkedAccountDetail,
                },
                new
                {
                    Patient = noNhsNumberEmisProxyPatient,
                    DemographicsResponse = demographicsResponseForNoNhsNumberUser,
                    LinkedAccountDetail = noNhsNumberLinkedAccountDetail,
                },
                new
                {
                    Patient = notMatchingOdsCodeEmisProxyPatient,
                    DemographicsResponse = demographicsResponseForNotMatchingOdsCodeUser,
                    LinkedAccountDetail = notMatchingOdsCodeLinkedAccountDetail,
                },
            };

            foreach (var patientData in groupedPatientData)
            {
                patientData.Patient.NhsNumber.Should().Be(patientData.DemographicsResponse.NhsNumber);
                patientData.LinkedAccountDetail.Id.Should().Be(patientData.Patient.Id);
                patientData.LinkedAccountDetail.Name.Should().Be(patientData.DemographicsResponse.PatientName);
                patientData.LinkedAccountDetail.GivenName.Should().Be(patientData.DemographicsResponse.NameParts.Given);
            }

            _demographicsService.VerifyAll();
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsEmptyResponse_WhenHasLinkedAccountsIsFalse()
        {
            _emisUserSession.HasLinkedAccounts = false;

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.LinkedAccountsBreakdown.Should().NotBeNull();
            successResult.LinkedAccountsBreakdown.ValidAccounts.Count().Should().Be(0);
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
            successResult.LinkedAccountsBreakdown.Should().NotBeNull();
            successResult.LinkedAccountsBreakdown.ValidAccounts.Count().Should().Be(0);
            _demographicsService.VerifyAll();
        }



        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdFoundInProxyUser()
        {
            //Arrange
            var patientId = Guid.NewGuid();
            var proxyNhsNumber = _fixture.Create<string>();

            _emisUserSession = new EmisUserSession
            {
                Id = Guid.NewGuid(),
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new EmisProxyUserSession
                    {
                        Id = patientId,
                        NhsNumber = proxyNhsNumber
                    },
                }
            };

            //Act
            var result = _systemUnderTest.GetProxyAuditData(_emisUserSession, patientId);

            //Assert
            result.IsProxyMode.Should().Be(true);
            result.ProxyNhsNumber.Should().Be(proxyNhsNumber);
        }


        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdFoundInMainUser()
        {
            //Arrange
            var patientId = Guid.NewGuid();

            _emisUserSession = new EmisUserSession
            {
                Id = patientId,
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            //Act
            var result = _systemUnderTest.GetProxyAuditData(_emisUserSession, patientId);

            //Assert
            result.IsProxyMode.Should().Be(false);
            result.ProxyNhsNumber.Should().Be(null);
        }

        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdNotFound()
        {
            //Arrange
            var patientId = Guid.NewGuid();

            _emisUserSession = new EmisUserSession
            {
                Id = Guid.NewGuid(),
                ProxyPatients = new List<EmisProxyUserSession>
                {
                    new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new EmisProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            //Act
            var result = _systemUnderTest.GetProxyAuditData(_emisUserSession, patientId);

            //Assert
            result.IsProxyMode.Should().Be(false);
            result.ProxyNhsNumber.Should().Be(null);
        }
    }
}