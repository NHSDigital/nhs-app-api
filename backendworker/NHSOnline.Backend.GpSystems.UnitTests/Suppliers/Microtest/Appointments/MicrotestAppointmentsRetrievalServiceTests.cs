using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using AppointmentsGetResponse = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments.AppointmentsGetResponse;
using IAppointmentsResponseMapper = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments.IAppointmentsResponseMapper;


namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class MicrotestAppointmentsRetrievalServiceTests
    {
        private IFixture _fixture;
        private Mock<IMicrotestClient> _mockMicrotestClient;
        private MicrotestUserSession _microtestUserSession;
        private MicrotestAppointmentsRetrievalService _systemUnderTest;
        private AppointmentsGetResponse _microtestClientGetResponse;
        private Mock<IAppointmentsResponseMapper> _mockResponseMapper;
        private AppointmentsResponse _mappedResponse;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _microtestUserSession = _fixture.Create<MicrotestUserSession>();
            _mockMicrotestClient = _fixture.Freeze<Mock<IMicrotestClient>>();
            _microtestClientGetResponse = _fixture.Create<AppointmentsGetResponse>();
            var response = new MicrotestClient.MicrotestApiObjectResponse<AppointmentsGetResponse>(HttpStatusCode.OK)
            {
                Body = _microtestClientGetResponse
            };

            _mockMicrotestClient
                .Setup(x => x.AppointmentsGet(
                        It.Is<string>( o => o.Equals(_microtestUserSession.OdsCode, StringComparison.Ordinal)),
                        It.Is<string>(n => n.Equals(_microtestUserSession.NhsNumber, StringComparison.Ordinal))
                    )
                )
                .ReturnsAsync(response);

            _mappedResponse = _fixture.Create<AppointmentsResponse>();
            _mockResponseMapper = _fixture.Freeze<Mock<IAppointmentsResponseMapper>>();
            _mockResponseMapper.Setup(x => x.Map(_microtestClientGetResponse))
                .Returns(_mappedResponse);

            _systemUnderTest = _fixture.Create<MicrotestAppointmentsRetrievalService>();
        }

        [TestMethod]
        public async Task GetAppointments_HappyPath_ReturnsSuccessResponse()
        {
            // Act
            var result = await _systemUnderTest.GetAppointments(_microtestUserSession);

            // Assert
            var response = result.Should().BeAssignableTo<AppointmentsResult.Success>().Subject.Response;
            response.Should().BeEquivalentTo(_mappedResponse);

            _mockMicrotestClient.VerifyAll();
            _mockResponseMapper.VerifyAll();
        }

        [TestMethod]
        public async Task GetAppointments_MicrotestClientReturnsForbidden_ReturnsForbiddenResponse()
        {
            // Arrange
            var response = new MicrotestClient.MicrotestApiObjectResponse<AppointmentsGetResponse>(HttpStatusCode.Forbidden);
            
            _mockMicrotestClient
                .Setup(x => x.AppointmentsGet(
                        It.Is<string>( o => o.Equals(_microtestUserSession.OdsCode, StringComparison.Ordinal)),
                        It.Is<string>(n => n.Equals(_microtestUserSession.NhsNumber, StringComparison.Ordinal))
                    )
                )
                .ReturnsAsync(response);
            
            // Act
            var result = await _systemUnderTest.GetAppointments(_microtestUserSession);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.Forbidden>();
        }
        
        [TestMethod]
        public async Task GetAppointments_MapperThrows_ReturnsInternalServerError()
        {
            // Arrange
            _mockResponseMapper.Setup(x => x.Map(_microtestClientGetResponse))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetAppointments(_microtestUserSession);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetAppointments_MicrotestClientThrows_ReturnsBadGateway()
        {
            // Arrange
            _mockMicrotestClient
                .Setup(x => x.AppointmentsGet(
                        It.Is<string>(o => o.Equals(_microtestUserSession.OdsCode, StringComparison.Ordinal)),
                        It.Is<string>(n => n.Equals(_microtestUserSession.NhsNumber, StringComparison.Ordinal))
                    )
                )
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAppointments(_microtestUserSession);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetAppointments_MicrotestClientReturnsUnsuccessfulStatusCode_ReturnsBadGateway()
        {
            // Arrange

            var response =
                new MicrotestClient.MicrotestApiObjectResponse<AppointmentsGetResponse>(HttpStatusCode.BadRequest);

            _mockMicrotestClient
                .Setup(x => x.AppointmentsGet(
                        It.Is<string>(o => o.Equals(_microtestUserSession.OdsCode, StringComparison.Ordinal)),
                        It.Is<string>(n => n.Equals(_microtestUserSession.NhsNumber, StringComparison.Ordinal))
                    )
                )
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.GetAppointments(_microtestUserSession);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.BadGateway>();
        }
    }
}
