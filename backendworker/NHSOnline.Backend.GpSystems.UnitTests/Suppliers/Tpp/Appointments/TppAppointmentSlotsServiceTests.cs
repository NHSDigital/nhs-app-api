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
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class TppAppointmentSlotsServiceTests
    {
        private IFixture _fixture;
        private Mock<ITppClient> _mockTppClient;
        private Mock<IAppointmentSlotsMapper> _mockSlotsMapper;
        private TppAppointmentSlotsService _systemUnderTest;
        private Mock<ICurrentDateTimeProvider> _mockCurrentDateTimeProvider;
        private TppUserSession _tppUserSession;
        private DateTimeOffset _fromDateTimeOffset;
        private DateTimeOffset _toDateTimeOffset;
        private AppointmentSlotsDateRange _dateRange;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockCurrentDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(DateTime.UtcNow);
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);
            
            _mockTppClient = _fixture.Freeze<Mock<ITppClient>>();
            _mockSlotsMapper = _fixture.Freeze<Mock<IAppointmentSlotsMapper>>();

            _tppUserSession = _fixture.Create<TppUserSession>();
                
            _fromDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();
            _toDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();

            _dateRange = new AppointmentSlotsDateRange(_fromDateTimeOffset, _toDateTimeOffset);
            
            _systemUnderTest = _fixture.Create<TppAppointmentSlotsService>();
        }

        [TestMethod]
        public async Task GetSlots_TppListSlotsPostThrowsHttpRequestException_ReturnsBadGateway()
        {
            // Arrange
            _mockTppClient
                .Setup(x => x.ListSlotsPost(It.IsAny<ListSlots>(), _tppUserSession.Suid))
                .ThrowsAsync(new HttpRequestException())
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.GetSlots(_tppUserSession, _dateRange);
            
            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetSlots_TppListSlotsPostUnsuccessful_ReturnsBadGateway()
        {
            // Arrange
            var unsuccessfulResponse = _fixture
                .Build<TppClient.TppApiObjectResponse<ListSlotsReply>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();
            
            MockTppClientListSlotsPost(unsuccessfulResponse);
            
            MockTppClientRequestSystmOnlineMessagesPost(new TppClient.TppApiObjectResponse<RequestSystmOnlineMessagesReply>(HttpStatusCode.OK));
            
            // Act
            var result = await _systemUnderTest.GetSlots(_tppUserSession, _dateRange);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetSlots_TppRequestSystmOnlineMessagesThrowsHttpRequestException_ReturnsSuccessfullyAnyway()
        {
            // Arrange
            _mockTppClient
                .Setup(x => x.RequestSystmOnlineMessages(It.IsAny<RequestSystmOnlineMessages>(), It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException())
                .Verifiable();
            
            MockTppClientListSlotsPost(new TppClient.TppApiObjectResponse<ListSlotsReply>(HttpStatusCode.OK));
            
            // Act
            var result = await _systemUnderTest.GetSlots(_tppUserSession, _dateRange);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Success>();
        }

        [TestMethod]
        public async Task GetSlots_TppRequestSystmOnlineMessagesError_ReturnsSuccessfullyAnyway()
        {
            // Arrange
            var unsuccessfulResponse = _fixture
                .Build<TppClient.TppApiObjectResponse<RequestSystmOnlineMessagesReply>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();
            
            MockTppClientRequestSystmOnlineMessagesPost(unsuccessfulResponse);
            
            MockTppClientListSlotsPost(new TppClient.TppApiObjectResponse<ListSlotsReply>(HttpStatusCode.OK));
            
            // Act
            var result = await _systemUnderTest.GetSlots(_tppUserSession, _dateRange);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Success>();
        }

        [TestMethod]
        public async Task GetSlots_TppListSlotsPostForbidden_ReturnsForbidden()
        {
            // Arrange
            var forbiddenResponse = _fixture
                .Build<TppClient.TppApiObjectResponse<ListSlotsReply>>()
                .With(x => x.StatusCode, HttpStatusCode.OK)
                .With(x => x.Body, null)
                .With(x => x.ErrorResponse, new Error { ErrorCode = TppApiErrorCodes.NoAccess })
                .Create();
            
            MockTppClientListSlotsPost(forbiddenResponse);
            
            MockTppClientRequestSystmOnlineMessagesPost(new TppClient.TppApiObjectResponse<RequestSystmOnlineMessagesReply>(HttpStatusCode.OK));
            
            // Act
            var result = await _systemUnderTest.GetSlots(_tppUserSession, _dateRange);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetSlots_ErrorsDuringMappingResponse_ReturnsInternalServerError()
        {
            // Arrange
            var listSlotsReply = new TppClient.TppApiObjectResponse<ListSlotsReply>(HttpStatusCode.OK);
            var messagesReply = new TppClient.TppApiObjectResponse<RequestSystmOnlineMessagesReply>(HttpStatusCode.OK);
            
            MockTppClientListSlotsPost(listSlotsReply);
            MockTppClientRequestSystmOnlineMessagesPost(messagesReply);

            _mockSlotsMapper.Setup(x => x.Map(listSlotsReply.Body, messagesReply.Body))
                .Throws<Exception>();
            
            // Act
            var result = await _systemUnderTest.GetSlots(_tppUserSession, _dateRange);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetSlots_HappyPath_ReturnsAppointmentSlots()
        {
            // Arrange
            var listSlotsReply = new TppClient.TppApiObjectResponse<ListSlotsReply>(HttpStatusCode.OK);
            var messagesReply = new TppClient.TppApiObjectResponse<RequestSystmOnlineMessagesReply>(HttpStatusCode.OK);
            
            MockTppClientListSlotsPost(listSlotsReply);
            MockTppClientRequestSystmOnlineMessagesPost(messagesReply);

            var expectedResponse = _fixture.Create<AppointmentSlotsResponse>();

            _mockSlotsMapper.Setup(x => x.Map(listSlotsReply.Body, messagesReply.Body))
                .Returns(expectedResponse);
            
            // Act
            var result = await _systemUnderTest.GetSlots(_tppUserSession, _dateRange);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Success>();
        }

        private void MockTppClientListSlotsPost(TppClient.TppApiObjectResponse<ListSlotsReply> response)
        {
            var expectedRequest = new ListSlots(_tppUserSession, _dateRange);

            _mockTppClient
                .Setup(x => x.ListSlotsPost(
                    It.Is<ListSlots>(p => 
                        p.PatientId.Equals(expectedRequest.PatientId, StringComparison.Ordinal)
                        && p.OnlineUserId.Equals(expectedRequest.OnlineUserId, StringComparison.Ordinal)
                        && p.StartDate.Equals(expectedRequest.StartDate, StringComparison.Ordinal)
                        && p.NumberOfDays.Equals(expectedRequest.NumberOfDays)
                        ),
                    _tppUserSession.Suid))
                .ReturnsAsync(response)
                .Verifiable();
        }

        private void MockTppClientRequestSystmOnlineMessagesPost(TppClient.TppApiObjectResponse<RequestSystmOnlineMessagesReply> response)
        {
            var expectedRequest = new RequestSystmOnlineMessages(_tppUserSession);
            
            _mockTppClient
                .Setup(x => x.RequestSystmOnlineMessages(
                    It.Is<RequestSystmOnlineMessages>(p => 
                        p.PatientId.Equals(expectedRequest.PatientId, StringComparison.Ordinal)
                        && p.OnlineUserId.Equals(expectedRequest.OnlineUserId, StringComparison.Ordinal)
                        ),
                    _tppUserSession.Suid))
                .ReturnsAsync(response)
                .Verifiable();
        }
    }
}