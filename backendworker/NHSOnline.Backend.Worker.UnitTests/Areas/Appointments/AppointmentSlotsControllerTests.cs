using System;
using System.Collections.Generic;
using System.Net;
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
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", "GMT Standard Time") });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider);

            _systemUnderTest = new AppointmentSlotsController(_gpSystemFactory.Object,
                _dateTimeOffsetProvider,
                _fixture.Create<ILoggerFactory>().CreateLogger<AppointmentSlotsController>(),
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
            var fromDate = _dateTimeOffsetProvider.CreateDateTimeOffset();
            var toDate = _dateTimeOffsetProvider.CreateDateTimeOffset().AddDays(28);
            var gpSystem = new Mock<IGpSystem>();
            var appointmentSlotsService = new Mock<IAppointmentSlotsService>();
            var appointmentSlotsServicesGetResponse = new AppointmentSlotsResponse();

            // Arrange
            _gpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(gpSystem.Object);

            gpSystem.Setup(x => x.GetAppointmentSlotsService())
                .Returns(appointmentSlotsService.Object);

            var successResponse = new AppointmentSlotsResult.SuccessfullyRetrieved(appointmentSlotsServicesGetResponse);

            appointmentSlotsService
                .Setup(x => x.GetSlots(_userSession, fromDate, toDate))
                .Returns(Task.FromResult((AppointmentSlotsResult)successResponse));

            // Act
            var queryParams = new PatientAppointmentSlotsQueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _systemUnderTest.Get(queryParams);

            // Assert
            _gpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            gpSystem.Verify(x => x.GetAppointmentSlotsService());
            appointmentSlotsService.Verify(x => x.GetSlots(_userSession, fromDate, toDate));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value;
            value.Should().BeAssignableTo(typeof(AppointmentSlotsResponse));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
        }
        
        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenQueryParametersAreInvalid()
        {
            var fromDate = _dateTimeOffsetProvider.CreateDateTimeOffset().AddDays(-14);
            var toDate = _dateTimeOffsetProvider.CreateDateTimeOffset().AddDays(-6);
            // Act
            var queryParams = new PatientAppointmentSlotsQueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _systemUnderTest.Get(queryParams);

            // Assert
            result.Should().BeAssignableTo(typeof(BadRequestResult));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
        }
        
        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenServiceReturnsBadRequest()
        {
            var fromDate = _dateTimeOffsetProvider.CreateDateTimeOffset();
            var toDate = _dateTimeOffsetProvider.CreateDateTimeOffset().AddDays(28);
            var gpSystem = new Mock<IGpSystem>();
            var appointmentSlotsService = new Mock<IAppointmentSlotsService>();

            var getAppointmentSlotsServiceResult = new AppointmentSlotsResult.BadRequest();

            // Arrange
            _gpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(gpSystem.Object);

            gpSystem.Setup(x => x.GetAppointmentSlotsService())
                .Returns(appointmentSlotsService.Object);

            appointmentSlotsService.Setup(x => x.GetSlots(_userSession, fromDate, toDate)).Returns(Task.FromResult((AppointmentSlotsResult)getAppointmentSlotsServiceResult));

            // Act
            var queryParams = new PatientAppointmentSlotsQueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _systemUnderTest.Get(queryParams);

            // Assert
            _gpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            gpSystem.Verify(x => x.GetAppointmentSlotsService());
            appointmentSlotsService.Verify(x => x.GetSlots(_userSession, fromDate, toDate));
            result.Should().BeAssignableTo(typeof(BadRequestResult));
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
        }
        
        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenServiceReturnsBadRequest()
        {
            var fromDate = _dateTimeOffsetProvider.CreateDateTimeOffset();
            var toDate = _dateTimeOffsetProvider.CreateDateTimeOffset().AddDays(28);
            var gpSystem = new Mock<IGpSystem>();
            var appointmentSlotsService = new Mock<IAppointmentSlotsService>();

            var getAppointmentSlotsServiceResult = new AppointmentSlotsResult.SupplierSystemUnavailable();

            // Arrange
            _gpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(gpSystem.Object);

            gpSystem.Setup(x => x.GetAppointmentSlotsService())
                .Returns(appointmentSlotsService.Object);

            appointmentSlotsService.Setup(x => x.GetSlots(_userSession, fromDate, toDate)).Returns(Task.FromResult((AppointmentSlotsResult)getAppointmentSlotsServiceResult));

            // Act
            var queryParams = new PatientAppointmentSlotsQueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _systemUnderTest.Get(queryParams);

            // Assert
            _gpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            gpSystem.Verify(x => x.GetAppointmentSlotsService());
            appointmentSlotsService.Verify(x => x.GetSlots(_userSession, fromDate, toDate));
            result.Should().BeAssignableTo(typeof(StatusCodeResult));

            var statusCodeResult = (StatusCodeResult) result;
            statusCodeResult.StatusCode.Should().Equals(HttpStatusCode.InternalServerError);
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, It.IsAny<string>(), It.IsAny<object[]>()));
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}
