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

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _tppUserSession = _fixture.Create<TppUserSession>();
            _gpSessionManager = _fixture.Freeze<Mock<IGpSessionManager>>();
            _fireAndForgetService = _fixture.Freeze<Mock<IFireAndForgetService>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<TppLinkedAccountsService>>>();
            _systemUnderTest = _fixture.Create<TppLinkedAccountsService>();
        }

        [TestMethod]
        public void GetOdsCodeForLinkedAccount_ReturnsOdsCodeOfMainUserInSession()
        {
            // Arrange
            var proxyId = Guid.NewGuid();

            _tppUserSession = new TppUserSession
            {
                OdsCode = "A12345",
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession()
                    {
                        Id = proxyId,
                    },
                }
            };
            var mainUserOdsCode = _tppUserSession.OdsCode;

            // Act
            var resultOdsCode = _systemUnderTest.GetOdsCodeForLinkedAccount(_tppUserSession, proxyId);

            // Assert
            resultOdsCode.Should().Be(mainUserOdsCode);
        }

        [TestMethod]
        public async Task SwitchAccount_ReturnsTrue_WhenLinkedAccountWithMatchingIdFoundInUserSessionForProxy()
        {
            // Arrange
            var proxyId = Guid.NewGuid();

            _tppUserSession = new TppUserSession
            {
                Id = Guid.NewGuid(),
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                    },
                    new TppProxyUserSession
                    {
                        Id = proxyId,
                    },
                }
            };

            var request = new GpLinkedAccountModel(_tppUserSession, proxyId);

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
            var result = await _systemUnderTest.SwitchAccount(request);
            await action(serviceProvider);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.Success>();
            _gpSessionManager.Verify();
        }

        [TestMethod]
        public async Task SwitchAccount_ReturnsTrue_WhenLinkedAccountWithMatchingIdFoundInUserSessionForMainUser()
        {
            // Arrange
            var mainUserGuid = Guid.NewGuid();

            _tppUserSession = new TppUserSession
            {
                Id = mainUserGuid,
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        Id = mainUserGuid,
                    },
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                    },
                }
            };

            _gpSessionManager.Setup(x => x.CloseSession(It.IsAny<TppUserSession>()))
                .ReturnsAsync(new CloseSessionResult.Success())
                .Verifiable();

            var request = new GpLinkedAccountModel(_tppUserSession, mainUserGuid);

            Func<IServiceProvider, Task> action = null;
            _fireAndForgetService.Setup(x => x.Run(It.IsAny<Func<IServiceProvider, Task>>(), It.IsAny<string>()))
                .Callback<Func<IServiceProvider, Task>, string>((a, m) => action = a);

            var serviceProvider = new ServiceCollection()
                .AddSingleton(_gpSessionManager.Object)
                .BuildServiceProvider();

            // Act
            var result = await _systemUnderTest.SwitchAccount(request);
            await action(serviceProvider);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.Success>();
            _gpSessionManager.Verify();
        }

        [TestMethod]
        public async Task SwitchAccount_ReturnsFalse_WhenLinkedAccountWithMatchingIdIsNotFoundInUserSession()
        {
            // Arrange
            var randomGuidWhichWontBeFound = Guid.NewGuid();

            _tppUserSession = new TppUserSession
            {
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                    },
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                    },
                }
            };

            var request = new GpLinkedAccountModel(_tppUserSession, randomGuidWhichWontBeFound);

            // Act
            var result = await _systemUnderTest.SwitchAccount(request);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.NotFound>();
            _gpSessionManager.Verify(x => x.CloseSession(_tppUserSession), Times.Never);
        }

        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdFoundInProxyUser()
        {
            //Arrange
            var patientId = Guid.NewGuid();
            var proxyNhsNumber = _fixture.Create<string>();

            _tppUserSession = new TppUserSession()
            {
                Id = Guid.NewGuid(),
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession()
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession()
                    {
                        Id = patientId,
                        NhsNumber = proxyNhsNumber
                    },
                }
            };

            //Act
            var result = _systemUnderTest.GetProxyAuditData(
                new GpLinkedAccountModel(_tppUserSession, patientId));

            //Assert
            result.IsProxyMode.Should().Be(true);
            result.ProxyNhsNumber.Should().Be(proxyNhsNumber);
        }


        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdFoundInMainUser()
        {
            //Arrange
            var patientId = Guid.NewGuid();

            _tppUserSession = new TppUserSession
            {
                Id = patientId,
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession()
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            //Act
            var result = _systemUnderTest.GetProxyAuditData(
                new GpLinkedAccountModel(_tppUserSession, patientId));

            //Assert
            result.IsProxyMode.Should().Be(false);
            result.ProxyNhsNumber.Should().Be(null);
        }

        [TestMethod]
        public void GetProxyAuditData_WhenPatientIdNotFound()
        {
            //Arrange
            var patientId = Guid.NewGuid();

            _tppUserSession = new TppUserSession
            {
                Id = Guid.NewGuid(),
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            //Act
            var result = _systemUnderTest.GetProxyAuditData(
                new GpLinkedAccountModel(_tppUserSession, patientId));

            //Assert
            result.IsProxyMode.Should().Be(false);
            result.ProxyNhsNumber.Should().Be(null);
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsEmptyResponse_WhenHasLinkedAccountsIsFalse()
        {
            _tppUserSession.ProxyPatients = new List<TppProxyUserSession>();

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_tppUserSession);

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
                Id = Guid.NewGuid(),
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        FullName = _fixture.Create<string>(),
                        DateOfBirth = _fixture.Create<DateTime>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        FullName = _fixture.Create<string>(),
                        DateOfBirth = _fixture.Create<DateTime>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        FullName = _fixture.Create<string>(),
                        DateOfBirth = _fixture.Create<DateTime>(),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            //Act
            var result = await _systemUnderTest.GetLinkedAccounts(_tppUserSession);

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

                linkedAccountDetail.Id.Should().Be(tppProxyPatient.Id);
                linkedAccountDetail.FullName.Should().Be(tppProxyPatient.FullName);
                linkedAccountDetail.AgeMonths.Should()
                    .Be(CalculateAge.CalculateAgeInMonthsAndYears(tppProxyPatient.DateOfBirth).AgeMonths);
                linkedAccountDetail.AgeYears.Should()
                    .Be(CalculateAge.CalculateAgeInMonthsAndYears(tppProxyPatient.DateOfBirth).AgeYears);
            }
        }

        [TestMethod]
        public async Task GetLinkedAccounts_ReturnsCorrectLinkedAccountsSummary()
        {
            // Arrange
            _tppUserSession = new TppUserSession
            {
                Id = Guid.NewGuid(),
                ProxyPatients = new List<TppProxyUserSession>
                {
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                    new TppProxyUserSession
                    {
                        Id = Guid.NewGuid(),
                        NhsNumber = _fixture.Create<string>()
                    },
                }
            };

            //Act
            var result = await _systemUnderTest.GetLinkedAccounts(_tppUserSession);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.ValidAccounts.Count().Should().Be(2);
        }

        [TestMethod]
        public async Task GetLinkedAccount_returns_InValidData()
        {
            var result = await _systemUnderTest.GetLinkedAccount(_tppUserSession, Guid.NewGuid());

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
                .Skip(ofWhichHaveDifferentAddress)
                .Skip(ofWhichShouldHaveNoNhsNumber);

            // Act
            var result = _systemUnderTest.ExtractValidProxyPatients(authenticateReply);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Information,
                $"User has linked_accounts={numberOfLinkedAccounts}, with different_ods_codes_to_user={ofWhichHaveDifferentAddress}", Times.Once());

            _mockLogger.VerifyLogger(
                LogLevel.Information,
                $"Linked_profiles_count={numberOfLinkedAccounts}, " +
                $"excluded_for_not_having_NHS_number={ofWhichShouldHaveNoNhsNumber}, " +
                $"excluding_for_having_different_ODS_code={ofWhichHaveDifferentAddress}, " +
                $"valid_and_being_returned: {expectedNumberOfValidPatients}",
                Times.Once());

            result.Count.Should().Be(expectedNumberOfValidPatients);
            result.Should().BeEquivalentTo(expectedValidPeople);
        }
    }
}