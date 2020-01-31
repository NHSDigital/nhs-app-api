using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
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
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private Mock<IAppointmentTypeTransformingVisitor> _mockAppointmentTypeTransformingVisitor;
        private string _serviceDeskReference;
        private AppointmentSlotsResponse _slotsResponse;
        private Guid _patientId;
        private AppointmentSlotsResult.Success _serviceResult;

        private const string RequestAuditType = "Appointments_GetSlots_Request";
        private const string ResponseAuditType = "Appointments_GetSlots_Response";

        private const string RequestAuditMessage = "Attempting to get available appointments";

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _userSession = _fixture.Create<UserSession>();

            _mockAppointmentSlotsService = _fixture.Freeze<Mock<IAppointmentSlotsService>>();

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<AppointmentSlotsController>>>();
            _mockAppointmentTypeTransformingVisitor = _fixture.Freeze<Mock<IAppointmentTypeTransformingVisitor>>();

            _slotsResponse = _fixture.Create<AppointmentSlotsResponse>();
            _serviceResult = new AppointmentSlotsResult.Success(_slotsResponse);

            _mockAppointmentSlotsService
                .Setup(x => x.GetSlots(
                    It.Is<GpLinkedAccountModel>(
                        g => g.GpUserSession ==_userSession.GpUserSession && g.PatientId == _patientId),
                    It.IsAny<AppointmentSlotsDateRange>()))
                .Returns(Task.FromResult((AppointmentSlotsResult)_serviceResult));

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

            _mockErrorReferenceGenerator = _fixture.Freeze<Mock<IErrorReferenceGenerator>>();
            _serviceDeskReference = _fixture.Create<string>();

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
            // Act
            var result = await _systemUnderTest.Get(_patientId);

            // Assert
            _mockAppointmentSlotsService.Verify(x => x.GetSlots(
                It.Is<GpLinkedAccountModel>(
                    g => g.GpUserSession ==_userSession.GpUserSession && g.PatientId == _patientId),
                It.IsAny<AppointmentSlotsDateRange>()));
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<AppointmentSlotsResponse>();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, $"Available appointment slots successfully viewed - {_slotsResponse.Slots.Count()} slots"));
        }

        [TestMethod]
        public async Task Get_ServiceReturnsResult_SlotTypesAreTransformed()
        {
            // Act
            await _systemUnderTest.Get(_patientId);

            // Assert
            _mockAppointmentTypeTransformingVisitor.Verify(x => x.Visit(_serviceResult));
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_LogsAppointmentSlotCount()
        {
            // Act
            await _systemUnderTest.Get(_patientId);

            // Assert
            var expectedLogMessage =
                $"Appointment Slot Count: Supplier=Emis OdsCode={_userSession.GpUserSession.OdsCode} " +
                $"Count={_slotsResponse.Slots.Count()}";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_LogsAppointmentSlotMetadataAfterSlotsHaveBeenTransformed()
        {
            // Arrange
            var originalFirstSlotType = _slotsResponse.Slots.First().Type;
            const string transformedFirstSlotType = "Transformed Slot Type";

            _mockAppointmentTypeTransformingVisitor.Setup(x => x.Visit(_serviceResult))
                .Callback<AppointmentSlotsResult.Success>(result =>
                {
                    result.Response.Slots.First().Type = transformedFirstSlotType;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.Get(_patientId);

            // Assert
            var slots = _slotsResponse.Slots;
            var locations = string.Join(",", slots.Select(x => "\"" + x.Location + "\""));
            var slotTypes = string.Join(",", slots.Select(x => "\"" + x.Type + "\""))
                .Replace(originalFirstSlotType, transformedFirstSlotType, StringComparison.OrdinalIgnoreCase);
            var slotTypesFromGpSystem = string.Join(",", slots.Select(x => "\"" + x.TypeFromGpSystem + "\""));
            var furthestSlotDays = (slots.Select(x => x.StartTime).Max().Date - DateTime.UtcNow.Date).Days;
            var expectedLogMessage =
                "appointment_slot_data={" +
                "\"Supplier\":\""+_userSession.GpUserSession.Supplier+"\"," +
                "\"OdsCode\":\""+_userSession.GpUserSession.OdsCode+"\","+
                "\"SlotTypes\":["+slotTypes+"]," +
                "\"SlotTypesFromGpSystem\":["+slotTypesFromGpSystem+"]," +
                "\"Locations\":["+locations+"]," +
                "\"SlotCount\":"+_slotsResponse.Slots.Count+"," +
                "\"FurthestSlotDays\":"+furthestSlotDays+"}";
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
            var result = await _systemUnderTest.Get(_patientId);

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
