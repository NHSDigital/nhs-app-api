using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Appointments;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Appointments
{
    [TestClass]
    public class AppointmentsSlotsControllerTests
    {
        private AppointmentSlotsController _systemUnderTest;
        private static IFixture _fixture;
        private Mock<ISystemProviderFactory> _systemProviderFactory;
        private UserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _systemProviderFactory = _fixture.Freeze<Mock<ISystemProviderFactory>>();
            _userSession = _fixture.Create<UserSession>();
            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

            _systemUnderTest = _fixture.Create<AppointmentSlotsController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
            
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var fromDate = new DateTime(2018, 4, 9).ToDateTimeOffset();
            var toDate = new DateTime(2018, 4, 24).ToDateTimeOffset();
            var systemProvider = new Mock<ISystemProvider>();
            var appointmentSlotsService = new Mock<IAppointmentSlotsService>();
            var appointmentSlotsServicesGetResponse = new AppointmentSlotsResponse();

            var getAppointmentSlotsServiceResult = new AppointmentSlotsResult.SuccessfullyRetrieved(appointmentSlotsServicesGetResponse);

            // Arrange
            _systemProviderFactory.Setup(x => x.CreateSystemProvider(_userSession.Supplier))
                .Returns(systemProvider.Object);

            systemProvider.Setup(x => x.GetAppointmentSlotsService())
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
            _systemProviderFactory.Verify(x => x.CreateSystemProvider(_userSession.Supplier));
            systemProvider.Verify(x => x.GetAppointmentSlotsService());
            appointmentSlotsService.Verify(x => x.Get(_userSession, fromDate, toDate));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value;
            value.Should().BeAssignableTo(typeof(AppointmentSlotsResponse));
        }
        
        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenQueryParametersAreInvalid()
        {
            var fromDate = new DateTime(2018, 4, 30).ToDateTimeOffset();
            var toDate = new DateTime(2018, 4, 24).ToDateTimeOffset();
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
            var fromDate = new DateTime(2018, 4, 9).ToDateTimeOffset();
            var toDate = new DateTime(2018, 4, 24).ToDateTimeOffset();
            var systemProvider = new Mock<ISystemProvider>();
            var appointmentSlotsService = new Mock<IAppointmentSlotsService>();

            var getAppointmentSlotsServiceResult = new AppointmentSlotsResult.BadRequest();

            // Arrange
            _systemProviderFactory.Setup(x => x.CreateSystemProvider(_userSession.Supplier))
                .Returns(systemProvider.Object);

            systemProvider.Setup(x => x.GetAppointmentSlotsService())
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
            _systemProviderFactory.Verify(x => x.CreateSystemProvider(_userSession.Supplier));
            systemProvider.Verify(x => x.GetAppointmentSlotsService());
            appointmentSlotsService.Verify(x => x.Get(_userSession, fromDate, toDate));
            result.Should().BeAssignableTo(typeof(BadRequestResult));
        }
        
        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenServiceReturnsBadRequest()
        {
            var fromDate = new DateTime(2018, 4, 9).ToDateTimeOffset();
            var toDate = new DateTime(2018, 4, 24).ToDateTimeOffset();
            var systemProvider = new Mock<ISystemProvider>();
            var appointmentSlotsService = new Mock<IAppointmentSlotsService>();

            var getAppointmentSlotsServiceResult = new AppointmentSlotsResult.SupplierSystemUnavailable();

            // Arrange
            _systemProviderFactory.Setup(x => x.CreateSystemProvider(_userSession.Supplier))
                .Returns(systemProvider.Object);

            systemProvider.Setup(x => x.GetAppointmentSlotsService())
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
            _systemProviderFactory.Verify(x => x.CreateSystemProvider(_userSession.Supplier));
            systemProvider.Verify(x => x.GetAppointmentSlotsService());
            appointmentSlotsService.Verify(x => x.Get(_userSession, fromDate, toDate));
            result.Should().BeAssignableTo(typeof(StatusCodeResult));

            var statusCodeResult = (StatusCodeResult) result;
            statusCodeResult.StatusCode.Should().Equals(HttpStatusCode.InternalServerError);
        }
    }
}
