using System;
using System.Collections.Generic;
using System.Linq;
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

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.LinkedAccounts
{
    [TestClass]
    public class LinkedAccountServiceTests
    {
        private EmisLinkedAccountsService _systemUnderTest;
        private Mock<IEmisDemographicsService> _demographicsService;
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
            
            _settings = new EmisConfigurationSettings(BaseUri, DefaultEmisApplicationId, DefaultEmisVersion, CertificatePath, 
                CertificatePassphrase, EmisExtendedHttpTimeoutSeconds, DefaultHttpTimeoutSeconds, CoursesMaxCoursesLimit, PrescriptionsMaxCoursesSoftLimit, 
                Environment);
            _fixture.Inject(_settings);
            _fixture.Inject(_logger);
            _fixture.Inject(_demographicsService);
            _systemUnderTest = _fixture.Create<EmisLinkedAccountsService>();
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessResponse_WhenSuccessfulResponseFromEmis()
        {
            _emisUserSession.HasLinkedAccounts = true;
            _emisUserSession.ProxyPatients = _fixture.Create<List<EmisProxyUserSession>>();

            foreach (var user in _emisUserSession.ProxyPatients)
            {
                DemographicsResult demographicsResult = new DemographicsResult.Success(new DemographicsResponse());

                _demographicsService
                    .Setup(x => x.GetDemographics(
                        It.Is<EmisUserSession>(e => string.Equals(e.SessionId, _emisUserSession.SessionId, StringComparison.Ordinal)
                        && string.Equals(e.EndUserSessionId, _emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                        && string.Equals(e.UserPatientLinkToken, user.UserPatientLinkToken, StringComparison.Ordinal))))
                    .Returns(Task.FromResult(demographicsResult))
                    .Verifiable();
            }

            // Act
            var result = await _systemUnderTest.GetLinkedAccounts(_emisUserSession);

            // Assert
            var successResult = result.Should().BeOfType<LinkedAccountsResult.Success>().Subject;
            successResult.Response.Should().NotBeNull();
            successResult.Response.LinkedAccounts.Count().Should().Be(_emisUserSession.ProxyPatients.Count);
            _demographicsService.VerifyAll();
        }
        
        [TestMethod]
        public async Task Get_ReturnsEmptyResponse_WhenHasLinkedAccountsIsFalse()
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
        public async Task Get_ReturnsEmptyResponse_WhenDemographicServiceReturnsNothing()
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