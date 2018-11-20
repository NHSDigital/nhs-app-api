using System;
using System.Collections.Generic;
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
using NHSOnline.Backend.Worker.Areas.Appointments;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Appointments
{
    [TestClass]
    public sealed class AppointmentsSlotsControllerTests : IDisposable
    {
        private AppointmentSlotsController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystemFactory> _gpSystemFactory;
        private UserSession _userSession;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private Mock<IAuditor> _mockAuditor;

        private const string RequestAuditType = "Appointments_GetSlots_Request";
        private const string ResponseAuditType = "Appointments_GetSlots_Response";

        private const string RequestAuditMessage = "Attempting to get available appointments";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _gpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _userSession = _fixture.Create<UserSession>();
            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider);

            _systemUnderTest = new AppointmentSlotsController(_gpSystemFactory.Object,
                _dateTimeOffsetProvider,
                new Mock<ILogger<AppointmentSlotsController>>().Object,
                _mockAuditor.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            var gpSystem = new Mock<IGpSystem>();
            var appointmentSlotsService = new Mock<IAppointmentSlotsService>();
            var appointmentSlotsServicesGetResponse = new AppointmentSlotsResponse();

            _gpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(gpSystem.Object);

            gpSystem.Setup(x => x.GetAppointmentSlotsService())
                .Returns(appointmentSlotsService.Object);

            var successResponse = new AppointmentSlotsResult.SuccessfullyRetrieved(appointmentSlotsServicesGetResponse);

            appointmentSlotsService
                .Setup(x => x.GetSlots(_userSession, It.IsAny<AppointmentSlotsDateRange>()))
                .Returns(Task.FromResult((AppointmentSlotsResult)successResponse));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _gpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            gpSystem.Verify(x => x.GetAppointmentSlotsService());
            appointmentSlotsService.Verify(x => x.GetSlots(_userSession,
                It.IsAny<AppointmentSlotsDateRange>()));
            var okObjectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeAssignableTo(typeof(AppointmentSlotsResponse));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "Available appointment slots successfully viewed"));
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenServiceReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var gpSystem = new Mock<IGpSystem>();
            var appointmentSlotsService = new Mock<IAppointmentSlotsService>();

            var getAppointmentSlotsServiceResult = new AppointmentSlotsResult.SupplierSystemUnavailable();
 
            _gpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(gpSystem.Object);

            gpSystem.Setup(x => x.GetAppointmentSlotsService())
                .Returns(appointmentSlotsService.Object);

            appointmentSlotsService.Setup(x => x.GetSlots(_userSession, It.IsAny<AppointmentSlotsDateRange>()))
                .Returns(Task.FromResult((AppointmentSlotsResult) getAppointmentSlotsServiceResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _gpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            gpSystem.Verify(x => x.GetAppointmentSlotsService());
            appointmentSlotsService.Verify(x => x.GetSlots(_userSession,
                It.IsAny<AppointmentSlotsDateRange>()));
            result.Should().BeAssignableTo(typeof(StatusCodeResult));

            var statusCodeResult = (StatusCodeResult) result;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
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
