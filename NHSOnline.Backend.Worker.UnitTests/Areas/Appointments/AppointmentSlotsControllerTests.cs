using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Date;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Appointments;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Appointments
{
    [TestClass]
    public class AppointmentsSlotsControllerTests
    {
        private AppointmentSlotsController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IBridgeFactory> _bridgeFactory;
        private UserSession _userSession;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _bridgeFactory = _fixture.Freeze<Mock<IBridgeFactory>>();
            _userSession = _fixture.Create<UserSession>();
            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);
            
            var timeZoneInfoProvider = new TimeZoneInfoProvider();
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider);

            _systemUnderTest = new AppointmentSlotsController(_bridgeFactory.Object, _dateTimeOffsetProvider, _fixture.Create<ILoggerFactory>());

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var fromDate = _dateTimeOffsetProvider.CreateDateTimeOffset();
            var toDate = _dateTimeOffsetProvider.CreateDateTimeOffset().AddDays(14);
            var bridge = new Mock<IBridge>();
            var appointmentSlotsService = new Mock<IAppointmentSlotsService>();
            var appointmentSlotsServicesGetResponse = new AppointmentSlotsResponse();

            var getAppointmentSlotsServiceResult = new AppointmentSlotsResult.SuccessfullyRetrieved(appointmentSlotsServicesGetResponse);

            // Arrange
            _bridgeFactory.Setup(x => x.CreateBridge(_userSession.Supplier))
                .Returns(bridge.Object);

            bridge.Setup(x => x.GetAppointmentSlotsService())
                .Returns(appointmentSlotsService.Object);

            appointmentSlotsService.Setup(x => x.Get(_userSession, fromDate, toDate)).Returns(Task.FromResult((AppointmentSlotsResult)getAppointmentSlotsServiceResult));

            // Act
            var queryParams = new PatientAppointmentSlotsQueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _systemUnderTest.Get(queryParams);

            // Assert
            _bridgeFactory.Verify(x => x.CreateBridge(_userSession.Supplier));
            bridge.Verify(x => x.GetAppointmentSlotsService());
            appointmentSlotsService.Verify(x => x.Get(_userSession, fromDate, toDate));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value;
            value.Should().BeAssignableTo(typeof(AppointmentSlotsResponse));
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
        }
        
        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenServiceReturnsBadRequest()
        {
            var fromDate = _dateTimeOffsetProvider.CreateDateTimeOffset();
            var toDate = _dateTimeOffsetProvider.CreateDateTimeOffset().AddDays(14);
            var bridge = new Mock<IBridge>();
            var appointmentSlotsService = new Mock<IAppointmentSlotsService>();

            var getAppointmentSlotsServiceResult = new AppointmentSlotsResult.BadRequest();

            // Arrange
            _bridgeFactory.Setup(x => x.CreateBridge(_userSession.Supplier))
                .Returns(bridge.Object);

            bridge.Setup(x => x.GetAppointmentSlotsService())
                .Returns(appointmentSlotsService.Object);

            appointmentSlotsService.Setup(x => x.Get(_userSession, fromDate, toDate)).Returns(Task.FromResult((AppointmentSlotsResult)getAppointmentSlotsServiceResult));

            // Act
            var queryParams = new PatientAppointmentSlotsQueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _systemUnderTest.Get(queryParams);

            // Assert
            _bridgeFactory.Verify(x => x.CreateBridge(_userSession.Supplier));
            bridge.Verify(x => x.GetAppointmentSlotsService());
            appointmentSlotsService.Verify(x => x.Get(_userSession, fromDate, toDate));
            result.Should().BeAssignableTo(typeof(BadRequestResult));
        }
        
        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenServiceReturnsBadRequest()
        {
            var fromDate = _dateTimeOffsetProvider.CreateDateTimeOffset();
            var toDate = _dateTimeOffsetProvider.CreateDateTimeOffset().AddDays(14);
            var bridge = new Mock<IBridge>();
            var appointmentSlotsService = new Mock<IAppointmentSlotsService>();

            var getAppointmentSlotsServiceResult = new AppointmentSlotsResult.SupplierSystemUnavailable();

            // Arrange
            _bridgeFactory.Setup(x => x.CreateBridge(_userSession.Supplier))
                .Returns(bridge.Object);

            bridge.Setup(x => x.GetAppointmentSlotsService())
                .Returns(appointmentSlotsService.Object);

            appointmentSlotsService.Setup(x => x.Get(_userSession, fromDate, toDate)).Returns(Task.FromResult((AppointmentSlotsResult)getAppointmentSlotsServiceResult));

            // Act
            var queryParams = new PatientAppointmentSlotsQueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _systemUnderTest.Get(queryParams);

            // Assert
            _bridgeFactory.Verify(x => x.CreateBridge(_userSession.Supplier));
            bridge.Verify(x => x.GetAppointmentSlotsService());
            appointmentSlotsService.Verify(x => x.Get(_userSession, fromDate, toDate));
            result.Should().BeAssignableTo(typeof(StatusCodeResult));

            var statusCodeResult = (StatusCodeResult) result;
            statusCodeResult.StatusCode.Should().Equals(HttpStatusCode.InternalServerError);
        }
    }
}
