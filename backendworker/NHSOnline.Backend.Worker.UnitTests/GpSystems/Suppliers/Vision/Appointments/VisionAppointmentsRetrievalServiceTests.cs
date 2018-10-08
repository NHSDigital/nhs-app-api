using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class VisionAppointmentsRetrievalServiceTests
    {
        private IFixture _fixture;
        private Mock<IVisionClient> _mockVisionClient;
        private VisionUserSession _userSession;
        private VisionAppointmentsRetrievalService _systemUnderTest;
        private VisionResponse<BookedAppointmentsResponse> _visionClientGetResponse;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _userSession = _fixture.Create<VisionUserSession>();
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _visionClientGetResponse = _fixture.Create<VisionResponse<BookedAppointmentsResponse>>();
            
            var response = new VisionClient.VisionApiObjectResponse<BookedAppointmentsResponse>(HttpStatusCode.OK)
            {
                RawResponse = new VisionResponseEnvelope<BookedAppointmentsResponse>
                {
                    Body = new VisionResponseBody<BookedAppointmentsResponse>
                    {
                        VisionResponse = _visionClientGetResponse
                    }
                }
            };
            
            MockVisionClientAppointmentsGetMethod(response);
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOS()) });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider);
            
            var appointmentMapper = new AppointmentMapper(dateTimeOffsetProvider);
            var cancellationReasonMapper = new CancellationReasonMapper();
            
            _systemUnderTest = new VisionAppointmentsRetrievalService(
                _fixture.Create<ILogger<VisionAppointmentsRetrievalService>>(),
                _mockVisionClient.Object,
                new BookedAppointmentsResponseMapper(appointmentMapper, cancellationReasonMapper));
        }
        
        [TestMethod]
        public async Task GetAppointments_HappyPath_ReturnsSuccessfullyRetrievedResponse()
        {
            // Arrange

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession);

            // Assert
            var response = result.Should().BeAssignableTo<AppointmentsResult.SuccessfullyRetrieved>().Subject.Response;

            response.Appointments.Should().BeEmpty();
            response.CancellationReasons.Should().NotBeEmpty();
            _mockVisionClient.VerifyAll();
        }
        
        [TestMethod]
        public async Task GetAppointments_VisionClientThrows_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockVisionClient.Setup(x => x.GetExistingAppointments(
                    It.Is<VisionConnectionToken>(p => 
                        p.RosuAccountId.Equals(_userSession.RosuAccountId, StringComparison.Ordinal)
                        && p.ApiKey.Equals(_userSession.ApiKey, StringComparison.Ordinal)),
                    _userSession.OdsCode,
                    _userSession.PatientId
                ))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task GetAppointments_MapperThrows_ReturnsInternalServerError()
        {
            // Arrange
            _mockVisionClient.Setup(x => x.GetExistingAppointments(
                It.Is<VisionConnectionToken>(p => 
                    p.RosuAccountId.Equals(_userSession.RosuAccountId, StringComparison.Ordinal)
                    && p.ApiKey.Equals(_userSession.ApiKey, StringComparison.Ordinal)),
                _userSession.OdsCode,
                _userSession.PatientId
            ))
            .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.InternalServerError>();
        }
        
        private void MockVisionClientAppointmentsGetMethod(
            VisionClient.VisionApiObjectResponse<BookedAppointmentsResponse> response)
        {   
            _mockVisionClient.Setup(x => x.GetExistingAppointments(
                    It.Is<VisionConnectionToken>(p => 
                        p.RosuAccountId.Equals(_userSession.RosuAccountId, StringComparison.Ordinal)
                        && p.ApiKey.Equals(_userSession.ApiKey, StringComparison.Ordinal)),
                    _userSession.OdsCode,
                    _userSession.PatientId
                    ))
                .ReturnsAsync(response);
        }
    }
}