using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Session
{    
  [TestClass]
    public class TppLogMessagingTests
    {
        private const string ResponseSuidHeader = "suid";

        private ServiceProvider _serviceProvider;

        private TppLogMessagingService _systemUnderTest;

        private Mock<ILogger<TppLogMessagingService>> TppMessagingLoggerServiceLogger => _serviceProvider.MockLogger<TppLogMessagingService>();
        private Mock<ITppClientRequest<TppUserSession, ListServiceAccessesReply>> _mockListServiceAccesses;


        [TestInitialize]
        public void TestInitialize()
        {
            var services = new ServiceCollection();
            services.RegisterTppSessionServices();
            services.AddMockLoggers();
            _mockListServiceAccesses = services.AddMock<ITppClientRequest<TppUserSession, ListServiceAccessesReply>>();
            _serviceProvider = services.BuildServiceProvider();
            _systemUnderTest = _serviceProvider.GetRequiredService<TppLogMessagingService>();
        }

        [TestMethod]
        public async Task FetchAndLogAccessInformation_WhenCalled_CallsListServiceAccessesPostWithTppUserSession()
        {
            // Arrange
            var userSession = new TppUserSession();

            var listServicesAccessesResponse = ListServiceAccessesReply();

            _mockListServiceAccesses
                .Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => listServicesAccessesResponse);

            // Act
            await _systemUnderTest.FetchAndLogAccessInformation(userSession);

            // Assert
            _mockListServiceAccesses.Verify(x => x.Post(userSession));
        }

        [TestMethod]
        public async Task FetchAndLogAccessInformation_WhenServiceAccessDescriptionIsMessaging_Logs()
        {
            // Arrange
            var userSession = new TppUserSession { OdsCode = "ods"};

            var listServicesAccessesResponse = ListServiceAccessesReply(
                new ServiceAccess { Description = "Messaging", Status = "A", StatusDesc = "Enabled" });

            _mockListServiceAccesses
                .Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => listServicesAccessesResponse);

            // Act
            await _systemUnderTest.FetchAndLogAccessInformation(userSession);

            // Assert
            TppMessagingLoggerServiceLogger.VerifyLogger(
                LogLevel.Information,
                "ODSCode ods  PFS messaging enabled: A with description: Enabled",
                Times.Once());
        }

        [TestMethod]
        [DataRow("ods code", "ODSCode ods code  PFS messaging enabled: A with description: Enabled")]
        [DataRow("1234567890", "ODSCode 1234567890  PFS messaging enabled: A with description: Enabled")]
        public async Task FetchAndLogAccessInformation_WhenServiceAccessDescriptionIsMessaging_LogsOdsCode(string odsCode, string message)
        {
            // Arrange
            var userSession = new TppUserSession { OdsCode = odsCode };

            var listServicesAccessesResponse = ListServiceAccessesReply(
                new ServiceAccess { Description = "Messaging", Status = "A", StatusDesc = "Enabled" });

            _mockListServiceAccesses
                .Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => listServicesAccessesResponse);

            // Act
            await _systemUnderTest.FetchAndLogAccessInformation(userSession);

            // Assert
            TppMessagingLoggerServiceLogger.VerifyLogger(
                LogLevel.Information,
                message,
                Times.Once());
        }

        [TestMethod]
        [DataRow("A", "ODSCode ods  PFS messaging enabled: A with description: Enabled")]
        [DataRow("Z", "ODSCode ods  PFS messaging enabled: Z with description: Enabled")]
        public async Task FetchAndLogAccessInformation_WhenServiceAccessDescriptionIsMessaging_LogsStatus(string status, string message)
        {
            // Arrange
            var userSession = new TppUserSession { OdsCode = "ods" };

            var listServicesAccessesResponse = ListServiceAccessesReply(
                new ServiceAccess { Description = "Messaging", Status = status, StatusDesc = "Enabled" });

            _mockListServiceAccesses
                .Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => listServicesAccessesResponse);

            // Act
            await _systemUnderTest.FetchAndLogAccessInformation(userSession);

            // Assert
            TppMessagingLoggerServiceLogger.VerifyLogger(
                LogLevel.Information,
                message,
                Times.Once());
        }

        [TestMethod]
        [DataRow("Enabled", "ODSCode ods  PFS messaging enabled: A with description: Enabled")]
        [DataRow("Disabled", "ODSCode ods  PFS messaging enabled: A with description: Disabled")]
        public async Task FetchAndLogAccessInformation_WhenServiceAccessDescriptionIsMessaging_LogsStatusDescription(string statusDesc, string message)
        {
            // Arrange
            var userSession = new TppUserSession { OdsCode = "ods" };

            var listServicesAccessesResponse = ListServiceAccessesReply(
                new ServiceAccess { Description = "Messaging", Status = "A", StatusDesc = statusDesc });

            _mockListServiceAccesses
                .Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => listServicesAccessesResponse);

            // Act
            await _systemUnderTest.FetchAndLogAccessInformation(userSession);

            // Assert
            TppMessagingLoggerServiceLogger.VerifyLogger(
                LogLevel.Information,
                message,
                Times.Once());
        }

        [TestMethod]
        [DataRow("PaperTrail")]
        [DataRow("OtherDescription")]
        public async Task FetchAndLogAccessInformation_WhenServiceAccessDescriptionIsNotMessaging_LogsNothing(string description)
        {
            // Arrange
            var userSession = new TppUserSession();

            var listServicesAccessesResponse = ListServiceAccessesReply(
                new ServiceAccess { Description = description, Status = "A", StatusDesc = "Not Enabled" }
            );

            _mockListServiceAccesses
                .Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => listServicesAccessesResponse);

            // Act
            await _systemUnderTest.FetchAndLogAccessInformation(userSession);

            // Assert
            TppMessagingLoggerServiceLogger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()),
                Times.Never);
        }

        private static TppApiObjectResponse<ListServiceAccessesReply> ListServiceAccessesReply(params ServiceAccess[] serviceAccesses)
            => new TppApiObjectResponse<ListServiceAccessesReply>(HttpStatusCode.OK)
            {
                Body = new ListServiceAccessesReply { ServiceAccess = serviceAccesses.ToList() },
                Headers = new Dictionary<string, string> { { ResponseSuidHeader, "suid" } }
            };
    }
}