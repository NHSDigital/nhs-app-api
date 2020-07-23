using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
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
using NHSOnline.Backend.Support.Session;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Appointments
{
    [TestClass]
    public sealed class AppointmentsSlotsControllerTests : IDisposable
    {
        private AppointmentSlotsController _systemUnderTest;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IAppointmentSlotsService> _mockAppointmentSlotsService;
        private P9UserSession _userSession;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private Mock<IAuditor> _mockAuditor;
        private Mock<ILogger<AppointmentSlotsController>> _mockLogger;
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private Mock<IAppointmentTypeTransformingVisitor> _mockAppointmentTypeTransformingVisitor;
        private string _serviceDeskReference;
        private AppointmentSlotsResponse _slotsResponse;
        private Guid _patientId;
        private AppointmentSlotsResult.Success _serviceResult;
        private AppointmentsConfigurationSettings _settings;

        private const string RequestAuditType = "Appointments_GetSlots_Request";
        private const string ResponseAuditType = "Appointments_GetSlots_Response";

        private const string RequestAuditMessage = "Attempting to get available appointments";

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();

            _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(), new EmisUserSession(), "im1token");

            _mockAppointmentSlotsService = new Mock<IAppointmentSlotsService>();

            _mockAuditor = new Mock<IAuditor>();
            _mockLogger = new Mock<ILogger<AppointmentSlotsController>>();
            _mockAppointmentTypeTransformingVisitor = new Mock<IAppointmentTypeTransformingVisitor>();

            _slotsResponse = new AppointmentSlotsResponse();
            _serviceResult = new AppointmentSlotsResult.Success(_slotsResponse);

            _mockAppointmentSlotsService
                .Setup(x => x.GetSlots(
                    It.Is<GpLinkedAccountModel>(
                        g => g.GpUserSession ==_userSession.GpUserSession && g.PatientId == _patientId),
                    It.IsAny<AppointmentSlotsDateRange>()))
                .Returns(Task.FromResult((AppointmentSlotsResult)_serviceResult));

            var mockCurrentDateTimeProvider = new Mock<ICurrentDateTimeProvider>();
            mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(DateTime.UtcNow);

            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider, mockCurrentDateTimeProvider.Object);

            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem
                .Setup(x => x.GetAppointmentSlotsService())
                .Returns(_mockAppointmentSlotsService.Object);

            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);

            _mockErrorReferenceGenerator = new Mock<IErrorReferenceGenerator>();
            _serviceDeskReference = "service desk ref";

            _settings = new AppointmentsConfigurationSettings(true);

            _systemUnderTest = new AppointmentSlotsController(
                _mockGpSystemFactory.Object,
                _dateTimeOffsetProvider,
                _mockLogger.Object,
                _mockAuditor.Object,
                new Mock<IAppointmentSlotMetadataLogger>().Object,
                _mockErrorReferenceGenerator.Object,
                _mockAppointmentTypeTransformingVisitor.Object,
                _settings);
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Act
            var result = await _systemUnderTest.Get(_patientId, _userSession);

            // Assert
            _mockAppointmentSlotsService.Verify(x => x.GetSlots(
                It.Is<GpLinkedAccountModel>(
                    g => g.GpUserSession ==_userSession.GpUserSession && g.PatientId == _patientId),
                It.IsAny<AppointmentSlotsDateRange>()));
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<AppointmentSlotsResponse>();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, $"Available appointment slots successfully viewed - {_slotsResponse.Slots.Count} slots"));
        }

        [TestMethod]
        public async Task Get_ServiceReturnsResult_SlotTypesAreTransformed()
        {
            // Act
            await _systemUnderTest.Get(_patientId, _userSession);

            // Assert
            _mockAppointmentTypeTransformingVisitor.Verify(x => x.Visit(_serviceResult));
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_LogsAppointmentSlotCount()
        {
            // Act
            await _systemUnderTest.Get(_patientId, _userSession);

            // Assert
            var expectedLogMessage =
                $"Appointment Slot Count: Supplier=Emis OdsCode={_userSession.GpUserSession.OdsCode} " +
                $"Count={_slotsResponse.Slots.Count}";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_LogsAppointmentSlotMetadataAfterSlotsHaveBeenTransformed()
        {
            // Arrange
            const string originalSlotType = "Original Slot Type";
            const string transformedFirstSlotType = "Transformed Slot Type";

            _slotsResponse.Slots.Add(new Slot { Type = originalSlotType, TypeFromGpSystem = originalSlotType, Location = "loc"});

            _mockAppointmentTypeTransformingVisitor.Setup(x => x.Visit(_serviceResult))
                .Callback<AppointmentSlotsResult.Success>(result =>
                {
                    result.Response.Slots.First().Type = transformedFirstSlotType;
                })
                .Returns(Task.CompletedTask);

            _userSession.GpUserSession.OdsCode = "ODS Code";

            // Act
            await _systemUnderTest.Get(_patientId, _userSession);

            // Assert
            var expectedLogMessage =
                "appointment_slot_data={" +
                "\"Supplier\":\"Emis\"," +
                "\"OdsCode\":\"ODS Code\","+
                "\"SlotTypes\":[\"Transformed Slot Type\"]," +
                "\"SlotTypesFromGpSystem\":[\"Original Slot Type\"]," +
                "\"Locations\":[\"loc\"]," +
                "\"SlotCount\":1";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [DataTestMethod]
        [DataRow(typeof(AppointmentSlotsResult.Forbidden), StatusCodes.Status403Forbidden,
            "Available appointment slots view unsuccessful due to not having permissions to book appointments")]
        [DataRow(typeof(AppointmentSlotsResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Available appointment slots view unsuccessful due to internal server error")]
        [DataRow(typeof(AppointmentSlotsResult.BadGateway), StatusCodes.Status502BadGateway,
            "Available appointment slots view unsuccessful due to supplier unavailable")]
        public async Task Get_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessage)
        {
            // Arrange
            var serviceResult = (AppointmentSlotsResult)Activator.CreateInstance(serviceResultType);
            _mockAppointmentSlotsService.Setup(x => x.GetSlots(
                    It.Is<GpLinkedAccountModel>(
                        g => g.GpUserSession ==_userSession.GpUserSession && g.PatientId == _patientId),
                    It.IsAny<AppointmentSlotsDateRange>()))
                .Returns(Task.FromResult(serviceResult));
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Appointments,
                    expectedStatusCode, _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Get(_patientId, _userSession);

            // Assert
            _mockAppointmentSlotsService.Verify(x => x.GetSlots(
                It.Is<GpLinkedAccountModel>(
                    g => g.GpUserSession ==_userSession.GpUserSession && g.PatientId == _patientId),
                It.IsAny<AppointmentSlotsDateRange>()));
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(expectedStatusCode);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, expectedAuditResponseMessage));
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}
