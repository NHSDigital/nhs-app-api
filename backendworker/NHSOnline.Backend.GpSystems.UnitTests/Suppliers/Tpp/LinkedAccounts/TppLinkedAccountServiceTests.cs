using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.LinkedAccounts;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.LinkedAccounts
{
    [TestClass]
    public class TppLinkedAccountServiceTests
    {
        private TppLinkedAccountsService _systemUnderTest;
        private TppUserSession _tppUserSession;
        private IFixture _fixture;
        private Mock<IGpSessionManager> _gpSessionManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _tppUserSession = _fixture.Create<TppUserSession>();
            _gpSessionManager = _fixture.Freeze<Mock<IGpSessionManager>>();

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

            // Act
            var result = await _systemUnderTest.SwitchAccount(_tppUserSession, proxyId);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.Success>();
            //TODO - add this back in when next story is complete
            //_gpSessionManager.Verify(x => x.CloseSession(_tppUserSession), Times.Once);

        }

        [TestMethod]
        public async Task SwitchAccount_ReturnsTrue_WhenLinkedAccountWithMatchingIdFoundInUserSessionForMainUser()
        {
            // Arrange
            var mainUserGuid = Guid.NewGuid();

            _tppUserSession = new TppUserSession
            {
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

            // Act
            var result = await _systemUnderTest.SwitchAccount(_tppUserSession, mainUserGuid);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.Success>();
            //TODO - add this back in when next story is complete
            //_gpSessionManager.Verify(x => x.CloseSession(_tppUserSession), Times.Once);
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

            // Act
            var result = await _systemUnderTest.SwitchAccount(_tppUserSession, randomGuidWhichWontBeFound);

            // Assert
            result.Should().BeOfType<SwitchAccountResult.Failure>();
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
            var result = _systemUnderTest.GetProxyAuditData(_tppUserSession, patientId);

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
            var result = _systemUnderTest.GetProxyAuditData(_tppUserSession, patientId);

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
            var result = _systemUnderTest.GetProxyAuditData(_tppUserSession, patientId);

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
            successResult.LinkedAccountsBreakdown.Should().NotBeNull();
            successResult.LinkedAccountsBreakdown.ValidAccounts.Count().Should().Be(0);
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
            successResult.LinkedAccountsBreakdown.Should().NotBeNull();
            successResult.LinkedAccountsBreakdown.ValidAccounts.Count().Should()
                .Be(_tppUserSession.ProxyPatients.Count);
            successResult.LinkedAccountsBreakdown.AccountsWithNoNhsNumber.Count().Should().Be(0);
            successResult.LinkedAccountsBreakdown.AccountsWithMismatchingOdsCode.Count().Should().Be(0);
            successResult.HasAnyProxyInfoBeenUpdatedInSession.Should().BeFalse();

            for (var i = 0; i < _tppUserSession.ProxyPatients.Count; i++)
            {
                var tppProxyPatient = _tppUserSession.ProxyPatients.ElementAt(i);
                var linkedAccountDetail = successResult.LinkedAccountsBreakdown.ValidAccounts.ElementAt(i);

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
                        NhsNumber = null
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
            successResult.LinkedAccountsBreakdown.AccountsWithNoNhsNumber.Count().Should().Be(1);
            successResult.LinkedAccountsBreakdown.AccountsWithMismatchingOdsCode.Count().Should().Be(0);
            successResult.LinkedAccountsBreakdown.ValidAccounts.Count().Should().Be(2);
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
    }
}