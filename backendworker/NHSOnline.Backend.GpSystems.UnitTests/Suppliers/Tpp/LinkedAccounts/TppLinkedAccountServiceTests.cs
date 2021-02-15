using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.LinkedAccounts
{
    [TestClass]
    public class TppLinkedAccountServiceTests
    {
        private TppLinkedAccountsService _systemUnderTest;
        private TppUserSession _tppUserSession;
        private IFixture _fixture;
        private Mock<IGpSessionManager> _gpSessionManager;
        private Mock<IFireAndForgetService> _fireAndForgetService;
        private Mock<ILogger<TppLinkedAccountsService>> _mockLogger;
        private Mock<ICurrentDateTimeProvider> _timeProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _tppUserSession = _fixture.Create<TppUserSession>();
            _gpSessionManager = _fixture.Freeze<Mock<IGpSessionManager>>();
            _fireAndForgetService = _fixture.Freeze<Mock<IFireAndForgetService>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<TppLinkedAccountsService>>>();
            _timeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            _systemUnderTest = _fixture.Create<TppLinkedAccountsService>();
        }

        [TestMethod]
        public void GetOdsCodeForLinkedAccount_ReturnsOdsCodeOfMainUserInSession()
        {
            // Arrange
            var proxyPatientId = _fixture.Create<string>();

            _tppUserSession = new TppUserSession
            {
                OdsCode = "A12345",
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession()
                    {
                        PatientId = proxyPatientId,
                    },
                }
            };
            var mainUserOdsCode = _tppUserSession.OdsCode;

            // Act
            var resultOdsCode = _systemUnderTest.GetOdsCodeForLinkedAccount(_tppUserSession, proxyPatientId);

            // Assert
            resultOdsCode.Should().Be(mainUserOdsCode);
        }

        [TestMethod]
        public async Task SwitchAccount_ReturnsSuccess_WhenLinkedAccountWithMatchingIdFoundInUserSessionForProxy()
        {
            // Arrange
            var proxyPatientId = _fixture.Create<string>();

            _tppUserSession = new TppUserSession
            {
                PatientId = _fixture.Create<string>(),
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                    },
                    new TppProxyUserSession
                    {
                        PatientId = proxyPatientId,
                    },
                }
            };

            _gpSessionManager.Setup(x => x.CloseSession(It.IsAny<TppUserSession>()))
                .ReturnsAsync(new CloseSessionResult.Success())
                .Verifiable();

            Func<IServiceProvider, Task> action = null;
            _fireAndForgetService.Setup(x => x.Run(It.IsAny<Func<IServiceProvider, Task>>(), It.IsAny<string>()))
                .Callback<Func<IServiceProvider, Task>, string>((a, m) => action = a);

            var serviceProvider = new ServiceCollection()
                .AddSingleton(_gpSessionManager.Object)
                .BuildServiceProvider();

            // Act
            var result = await _systemUnderTest.SwitchAccount(_tppUserSession, proxyPatientId);
            await action(serviceProvider);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.Success>();
            _gpSessionManager.Verify();
        }

        [TestMethod]
        public async Task SwitchAccount_ReturnsSuccess_WhenLinkedAccountWithMatchingIdFoundInUserSessionForMainUser()
        {
            // Arrange
            var mainUserPatientId = _fixture.Create<string>();

            _tppUserSession = new TppUserSession
            {
                PatientId = mainUserPatientId,
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        PatientId = mainUserPatientId,
                    },
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>()
                    },
                }
            };

            _gpSessionManager.Setup(x => x.CloseSession(It.IsAny<TppUserSession>()))
                .ReturnsAsync(new CloseSessionResult.Success())
                .Verifiable();

            Func<IServiceProvider, Task> action = null;
            _fireAndForgetService.Setup(x => x.Run(It.IsAny<Func<IServiceProvider, Task>>(), It.IsAny<string>()))
                .Callback<Func<IServiceProvider, Task>, string>((a, m) => action = a);

            var serviceProvider = new ServiceCollection()
                .AddSingleton(_gpSessionManager.Object)
                .BuildServiceProvider();

            // Act
            var result = await _systemUnderTest.SwitchAccount(_tppUserSession, mainUserPatientId);
            await action(serviceProvider);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.Success>();
            _gpSessionManager.Verify();
        }

        [TestMethod]
        public async Task SwitchAccount_ReturnsNotFound_WhenLinkedAccountWithMatchingIdIsNotFoundInUserSession()
        {
            // Arrange
            var randomIdentifierWhichWontBeFound = _fixture.Create<string>();

            _tppUserSession = new TppUserSession
            {
                PatientId = _fixture.Create<string>(),
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                    },
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                    },
                }
            };

            // Act
            var result = await _systemUnderTest.SwitchAccount(_tppUserSession, randomIdentifierWhichWontBeFound);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.NotFound>();
            _gpSessionManager.Verify(x => x.CloseSession(_tppUserSession), Times.Never);
        }

        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdFoundInProxyUser()
        {
            // Arrange
            var patientId = _fixture.Create<string>();
            var proxyNhsNumber = _fixture.Create<string>();

            _tppUserSession = new TppUserSession
            {
                PatientId = _fixture.Create<string>(),
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession
                    {
                        PatientId = patientId,
                        NhsNumber = proxyNhsNumber
                    },
                }
            };

            // Act
            var result = _systemUnderTest.GetProxyAuditData(
                new GpLinkedAccountModel(_tppUserSession, patientId));

            // Assert
            result.IsProxyMode.Should().Be(true);
            result.ProxyNhsNumber.Should().Be(proxyNhsNumber);
        }


        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdFoundInMainUser()
        {
            // Arrange
            var patientId = _fixture.Create<string>();

            _tppUserSession = new TppUserSession
            {
                PatientId = patientId,
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            // Act
            var result = _systemUnderTest.GetProxyAuditData(
                new GpLinkedAccountModel(_tppUserSession, patientId));

            // Assert
            result.IsProxyMode.Should().Be(false);
            result.ProxyNhsNumber.Should().Be(null);
        }

        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdNotFound()
        {
            // Arrange
            var patientIdWhichWontBeFound = _fixture.Create<string>();

            _tppUserSession = new TppUserSession
            {
                PatientId = _fixture.Create<string>(),
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            // Act
            var result = _systemUnderTest.GetProxyAuditData(
                new GpLinkedAccountModel(_tppUserSession, patientIdWhichWontBeFound));

            // Assert
            result.IsProxyMode.Should().Be(false);
            result.ProxyNhsNumber.Should().Be(null);
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsEmptyResponse_WhenHasLinkedAccountsIsFalse()
        {
            _tppUserSession.ProxyPatients = new List<TppProxyUserSession>();

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_tppUserSession, new Dictionary<Guid, string>());

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.Should().NotBeNull();
            successResult.ValidAccounts.Count().Should().Be(0);
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsSuccessResult_WhenHasLinkedAccountsIsTrueAndAllValidAccounts()
        {
            // Arrange
            _tppUserSession = new TppUserSession
            {
                PatientId = _fixture.Create<string>(),
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                        FullName = _fixture.Create<string>(),
                        DateOfBirth = new DateTime(1972, 04, 11, 0, 0, 0, DateTimeKind.Utc),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                        FullName = _fixture.Create<string>(),
                        DateOfBirth = new DateTime(1972, 04, 12, 0, 0, 0, DateTimeKind.Utc),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                        FullName = _fixture.Create<string>(),
                        DateOfBirth = new DateTime(1972, 04, 13, 0, 0, 0, DateTimeKind.Utc),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            _timeProvider.Setup(x => x.LocalNow).Returns(new DateTime(2021, 04, 12, 4, 32, 0, DateTimeKind.Utc));
            var guidToPatientIdMapping = CreateGuidToPatientIdMapping(_tppUserSession);

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_tppUserSession, guidToPatientIdMapping);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.Should().NotBeNull();
            successResult.ValidAccounts.Count().Should()
                .Be(_tppUserSession.ProxyPatients.Count);
            successResult.HasAnyProxyInfoBeenUpdatedInSession.Should().BeFalse();

            for (var i = 0; i < _tppUserSession.ProxyPatients.Count; i++)
            {
                var tppProxyPatient = _tppUserSession.ProxyPatients.ElementAt(i);
                var linkedAccountDetail = successResult.ValidAccounts.ElementAt(i);
                var patientSessionId = guidToPatientIdMapping
                    .First(x => x.Value == tppProxyPatient.PatientId).Key;

                linkedAccountDetail.Id.Should().Be(patientSessionId);
                linkedAccountDetail.FullName.Should().Be(tppProxyPatient.FullName);
                switch (i)
                {
                    case 0:
                        linkedAccountDetail.AgeMonths.Should().Be(0);
                        linkedAccountDetail.AgeYears.Should().Be(49);
                        break;
                    case 1:
                        linkedAccountDetail.AgeMonths.Should().Be(0);
                        linkedAccountDetail.AgeYears.Should().Be(49);
                        break;
                    case 2:
                        linkedAccountDetail.AgeMonths.Should().Be(0);
                        linkedAccountDetail.AgeYears.Should().Be(48);
                        break;
                    default:
                        Assert.Fail("Invalid patient index {0}", i);
                        break;
                }
            }
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsCorrectLinkedAccountsSummary()
        {
            // Arrange
            _tppUserSession = new TppUserSession
            {
                PatientId = _fixture.Create<string>(),
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession
                    {
                        PatientId = _fixture.Create<string>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            var guidToPatientIdMapping = CreateGuidToPatientIdMapping(_tppUserSession);

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_tppUserSession, guidToPatientIdMapping);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.ValidAccounts.Count().Should().Be(2);
        }

        [TestMethod]
        public async Task GetLinkedAccount_ReturnsResponseIndicatingInvalidData_WhenLinkedAccountNotFound()
        {
            var result = await _systemUnderTest.GetLinkedAccount(_tppUserSession, _fixture.Create<string>());

            var successResult = result.Should().BeOfType<LinkedAccountAccessSummaryResult.Success>().Subject;
            successResult.Response.IsValidData.Should().BeFalse();
            successResult.Response.CanBookAppointment.Should().BeFalse();
            successResult.Response.CanOrderRepeatPrescription.Should().BeFalse();
            successResult.Response.CanViewMedicalRecord.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(0, 0, 0, 0)]
        [DataRow(1, 0, 0, 1)]
        [DataRow(1, 1, 0, 0)]
        [DataRow(3, 1, 1, 1)]
        [DataRow(3, 2, 0, 1)]
        [DataRow(3, 0, 3, 0)]
        public void ExtractValidProxyPatients_ReturnsCorrectLinkedAccounts_AndLogsCorrectNumbersOfLinkedAccounts(
            int numberOfLinkedAccounts,
            int ofWhichHaveDifferentAddress,
            int ofWhichShouldHaveNoNhsNumber,
            int expectedNumberOfValidPatients)
        {
            // Arrange
            const string patientId = "abc";
            const string gpPracticeName = "gp practice 1";
            const string gpAddress = "1 street name, town name";
            var authenticateReply = new AuthenticateReply
            {
                OnlineUserId = patientId,
                User = new User
                {
                    Person = new Person
                    {
                        PatientId = patientId,
                        NationalId = new NationalId
                        {
                            Type = "NHS",
                            Value = Guid.NewGuid().ToString(),
                        }
                    },
                },
                Registration = new Registration
                {
                    PatientAccess = new List<PatientAccess>
                    {
                        new PatientAccess
                        {
                            PatientId = patientId,
                            SiteDetails = new SiteDetails
                            {
                                UnitName = gpPracticeName,
                                Address = new TppAddress
                                {
                                    Address = gpAddress,
                                },
                            },
                        },
                    },
                },
            };

            for (var i = 0; i < numberOfLinkedAccounts; i++)
            {
                // In a response from TPP, each proxy patient has a <Person> element and an associated <PatientAccess> element.
                var proxyPatientId = Guid.NewGuid().ToString();
                var linkedAccount = new PatientAccess
                {
                    SiteDetails = new SiteDetails
                    {
                        Address = new TppAddress
                        {
                            Address = gpAddress,
                        },
                        UnitName = gpPracticeName,
                    },
                    PatientId = proxyPatientId,
                };

                var proxyPatient = new Person
                {
                    PatientId = proxyPatientId,
                    NationalId = new NationalId
                    {
                        Type = "NHS",
                        Value = Guid.NewGuid().ToString(),
                    }
                };

                authenticateReply.Registration.PatientAccess.Add(linkedAccount);
                authenticateReply.People.Add(proxyPatient);
            }

            // clear address of number of x people who shouldn't match main user address
            authenticateReply.People.Take(ofWhichHaveDifferentAddress).ToList()
                .ForEach(x =>
                {
                    var associatedAddressElement =
                        authenticateReply.Registration.PatientAccess.First(pa =>
                            pa.PatientId == x.PatientId);
                    associatedAddressElement.SiteDetails.Address.Address = "<address not matching main user>";
                });

            // clear nhs number of next x people to have no nhs number
            authenticateReply.People
                .Skip(ofWhichHaveDifferentAddress)
                .Take(ofWhichShouldHaveNoNhsNumber).ToList()
                .ForEach(x => x.NationalId = null);

            var expectedValidPeople = authenticateReply.People
                .Where(x => x.NationalId != null).ToList();

            // Act
            var result = _systemUnderTest.ExtractValidProxyPatients(authenticateReply);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Information,
                $"User has linked_accounts={numberOfLinkedAccounts}, with different_ods_codes_to_user={ofWhichHaveDifferentAddress}", Times.Once());

            _mockLogger.VerifyLogger(
                LogLevel.Information,
                $"Linked_profiles_count={numberOfLinkedAccounts}, " +
                $"excluded_for_not_having_NHS_number={ofWhichShouldHaveNoNhsNumber}, " +
                $"has_different_ODS_code={ofWhichHaveDifferentAddress}, " +
                $"valid_and_being_returned: {expectedNumberOfValidPatients + ofWhichHaveDifferentAddress}",
                Times.Once());

            result.Count.Should().Be(expectedNumberOfValidPatients + ofWhichHaveDifferentAddress);
            result.Should().BeEquivalentTo(expectedValidPeople);
        }

        [TestMethod]
        public void ExtractValidProxyPatients_LogsWarningButContinues_WhenSelfPatientIsNull()
        {
            // Arrange
            const string patientId = "abc";
            var proxyPatientId = Guid.NewGuid().ToString();
            const string gpPracticeName = "gp practice 1";
            const string gpAddress = "1 street name, town name";

            var proxyPatient = new Person
            {
                PatientId = proxyPatientId,
                NationalId = new NationalId
                {
                    Type = "NHS",
                    Value = Guid.NewGuid().ToString(),
                }
            };

            // In a response from TPP, for proxy patient only, add <Person> element and an associated <PatientAccess> element.
            var authenticateReply = new AuthenticateReply
            {
                OnlineUserId = patientId,
                User = new User
                {
                    Person = new Person
                    {
                        PatientId = patientId,
                        NationalId = new NationalId
                        {
                            Type = "NHS",
                            Value = Guid.NewGuid().ToString(),
                        }
                    },
                },
                Registration = new Registration
                {
                    PatientAccess = new List<PatientAccess>
                    {
                        new PatientAccess
                        {
                            SiteDetails = new SiteDetails
                            {
                                Address = new TppAddress
                                {
                                    Address = gpAddress,
                                },
                                UnitName = gpPracticeName,
                            },
                            PatientId = proxyPatientId,
                        },
                    },
                },
                People = new List<Person>
                {
                    proxyPatient,
                }
            };

            // Act
            var result = _systemUnderTest.ExtractValidProxyPatients(authenticateReply);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Warning,
                $"TPP user details not found in {nameof(AuthenticateReply.Registration.PatientAccess)}", Times.Once());

            _mockLogger.VerifyLogger(LogLevel.Information,
                "User has linked_accounts=1, with different_ods_codes_to_user=0", Times.Once());

            _mockLogger.VerifyLogger(
                LogLevel.Information,
                "Linked_profiles_count=1, " +
                "excluded_for_not_having_NHS_number=0, " +
                "has_different_ODS_code=0, " +
                "valid_and_being_returned: 1",
                Times.Once());

            var expectedValidPeople = new List<Person> { proxyPatient };
            result.Count.Should().Be(1);
            result.Should().BeEquivalentTo(expectedValidPeople);
        }

        private static Dictionary<Guid, string> CreateGuidToPatientIdMapping(TppUserSession tppUserSession)
        {
            var guidToPatientIdMapping = new Dictionary<Guid, string>();

            foreach (var patientId in new [] { tppUserSession.PatientId }.Concat(
                tppUserSession.ProxyPatients.Select(x => x.PatientId)))
            {
                guidToPatientIdMapping.Add(Guid.NewGuid(), patientId);
            }

            return guidToPatientIdMapping;
        }
    }
}