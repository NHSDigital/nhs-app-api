using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support.Temporal;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Appointments
{
    [TestClass]
    public sealed class AppointmentsSlotsControllerTests : IDisposable
    {
        private AppointmentSlotsController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IAppointmentSlotsService> _mockAppointmentSlotsService;
        private UserSession _userSession;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private Mock<IAuditor> _mockAuditor;
        private Mock<ILogger<AppointmentSlotsController>> _mockLogger;

        private const string RequestAuditType = "Appointments_GetSlots_Request";
        private const string ResponseAuditType = "Appointments_GetSlots_Response";

        private const string RequestAuditMessage = "Attempting to get available appointments";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));
            
            _userSession = _fixture.Create<UserSession>();
            
            _mockAppointmentSlotsService = _fixture.Freeze<Mock<IAppointmentSlotsService>>();
            
            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<AppointmentSlotsController>>>();

            var mockCurrentDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(DateTime.UtcNow);

            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider, mockCurrentDateTimeProvider.Object);

            _fixture.Inject(_dateTimeOffsetProvider);
            
            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystem
                .Setup(x => x.GetAppointmentSlotsService())
                .Returns(_mockAppointmentSlotsService.Object);
            
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);
            
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_userSession);

            _systemUnderTest = _fixture.Create<AppointmentSlotsController>();
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            var slotsResponse = _fixture.Create<AppointmentSlotsResponse>();
            var successResponse = new AppointmentSlotsResult.Success(slotsResponse);

            _mockAppointmentSlotsService
                .Setup(x => x.GetSlots(_userSession.GpUserSession, It.IsAny<AppointmentSlotsDateRange>()))
                .Returns(Task.FromResult((AppointmentSlotsResult)successResponse));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockAppointmentSlotsService.Verify(x => x.GetSlots(_userSession.GpUserSession,
                It.IsAny<AppointmentSlotsDateRange>()));
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<AppointmentSlotsResponse>();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, $"Available appointment slots successfully viewed - {slotsResponse.Slots.Count()} slots"));
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_LogsAppointmentSlotCount()
        {
            // Arrange
            var slotsResponse = _fixture.Create<AppointmentSlotsResponse>();
            var successResponse = new AppointmentSlotsResult.Success(slotsResponse);

            _mockAppointmentSlotsService
                .Setup(x => x.GetSlots(_userSession.GpUserSession, It.IsAny<AppointmentSlotsDateRange>()))
                .Returns(Task.FromResult((AppointmentSlotsResult)successResponse));

            // Act
            await _systemUnderTest.Get();

            // Assert
            var expectedLogMessage =
                $"Appointment Slot Count: Supplier=Emis OdsCode={_userSession.GpUserSession.OdsCode} " + 
                $"Count={slotsResponse.Slots.Count()}";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_LogsAppointmentSlotMetadata()
        {
            // Arrange
            var slotsResponse = _fixture.Create<AppointmentSlotsResponse>();
            var successResponse = new AppointmentSlotsResult.Success(slotsResponse);

            _mockAppointmentSlotsService
                .Setup(x => x.GetSlots(_userSession.GpUserSession, It.IsAny<AppointmentSlotsDateRange>()))
                .Returns(Task.FromResult((AppointmentSlotsResult)successResponse));

            // Act
            await _systemUnderTest.Get();

            // Assert
            var locations = string.Join(",", slotsResponse.Slots.Select(x => "\"" + x.Location + "\""));
            var slotTypes = string.Join(",", slotsResponse.Slots.Select(x => "\"" + x.Type + "\""));
            var expectedLogMessage =
                "appointment_slot_data={\"SlotTypes\":["+slotTypes+"],"+
                "\"Locations\":["+locations+"]," +
                "\"SlotCount\":"+slotsResponse.Slots.Count()+"}";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenServiceReturnsBadGateway()
        {
            // Arrange
            _mockAppointmentSlotsService.Setup(x => x.GetSlots(_userSession.GpUserSession, It.IsAny<AppointmentSlotsDateRange>()))
                .Returns(Task.FromResult((AppointmentSlotsResult) new AppointmentSlotsResult.BadGateway()));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockAppointmentSlotsService.Verify(x => x.GetSlots(_userSession.GpUserSession,
                It.IsAny<AppointmentSlotsDateRange>()));
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Available appointment slots view unsuccessful due to supplier unavailable"));
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}
