using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.ResponseParsers;
using RichardSzalay.MockHttp;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest
{
    [TestClass]
    public sealed class MicrotestClientTests : IDisposable
    {
        public static readonly Uri BaseUri = new Uri("http://microtest_base_url/");

        private const string CertificatePath = "CertificatePath";
        private const string CertificatePassphrase = "CertificatePassphrase";
        private const int PrescriptionsLimit = 100;
        private const int CoursesLimit = 100;
        private IMicrotestClient _systemUnderTest;
        private MockHttpMessageHandler _mockHttpHandler;
        private MicrotestConfigurationSettings _configurationSettings;
        private MicrotestHttpClient _httpClient;
        private IFixture _fixture;
        private string _nhsNumber;
        private string _odsCode;
        private const string Environment = "environment";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IJsonResponseParser>(() => new JsonResponseParser());

            _mockHttpHandler = new MockHttpMessageHandler();
            
            _configurationSettings = new MicrotestConfigurationSettings(BaseUri, CertificatePath, CertificatePassphrase, Environment, PrescriptionsLimit, CoursesLimit);

            _httpClient = new MicrotestHttpClient(new HttpClient(_mockHttpHandler), _configurationSettings);

            _fixture.Inject(_configurationSettings);
            _fixture.Inject(_httpClient);

            _odsCode = _fixture.Create<string>();
            _nhsNumber = _fixture.CreateNhsNumberFormatted();

            _systemUnderTest = _fixture.Create<MicrotestClient>();
        }

        [TestMethod]
        public async Task AppointmentSlotsGet_ReturnsAppointmentsResponse_WhenValidlyRequested()
        {
            // Arrange
            var fromDate = new DateTime(2000, 1, 1);
            var toDate = new DateTime(2000, 1, 2);

            var expectedResponse = _fixture.Create<AppointmentSlotsGetResponse>();

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Get, "patient/appointment-slots?fromDate=2000-01-01&toDate=2000-01-02")
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            var dateRange = new AppointmentSlotsDateRange(fromDate, toDate);

            // Act
            var response = await _systemUnderTest.AppointmentSlotsGet(_odsCode, _nhsNumber, dateRange);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task AppointmentSlotsGet_ReturnsNhsUnparsableException_WhenResponseIsNotJson()
        {
            // Arrange
            var fromDate = new DateTime(2000, 1, 1);
            var toDate = new DateTime(2000, 1, 2);

            var nonJsonResponse = _fixture.Create<string>();

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Get, "patient/appointment-slots?fromDate=2000-01-01&toDate=2000-01-02")
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .Respond(HttpStatusCode.OK, "application/json", nonJsonResponse);

            var dateRange = new AppointmentSlotsDateRange(fromDate, toDate);

            // Act
            Func<Task> act = async () => { await _systemUnderTest.AppointmentSlotsGet(_odsCode, _nhsNumber, dateRange); };

            // Assert
            await act.Should().ThrowAsync<NhsUnparsableException>();
        }

        [TestMethod]
        public async Task AppointmentsGet_ReturnsAppointmentsGetResponse_WhenValidlyRequested()
        {
            // Arrange
            var expectedResponse = _fixture.Create<AppointmentsGetResponse>();

            var pastAppointmentsFromDate = _fixture.Create<DateTimeOffset>();
            var path = "patient/appointments?pastAppointmentsFromDate=" +
                       pastAppointmentsFromDate.ToString("yyyy-MM-dd",
                           System.Globalization.CultureInfo.InvariantCulture);

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Get, path)
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.AppointmentsGet(_odsCode, _nhsNumber, pastAppointmentsFromDate);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task AppointmentsGet_ReturnsNhsUnparsableException_WhenResponseIsNotJson()
        {
            // Arrange
            var nonJsonResponse = _fixture.Create<string>();

            var pastAppointmentsFromDate = _fixture.Create<DateTimeOffset>();
            var path = "patient/appointments?pastAppointmentsFromDate=" + 
                       pastAppointmentsFromDate.ToString("yyyy-MM-dd",
                                                                  System.Globalization.CultureInfo.InvariantCulture);

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Get, path)
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .Respond(HttpStatusCode.OK, "application/json", nonJsonResponse);

            // Act
            Func<Task> act = async () => { await _systemUnderTest.AppointmentsGet(_odsCode, _nhsNumber, pastAppointmentsFromDate); };

            // Assert
            await act.Should().ThrowAsync<NhsUnparsableException>();
        }

        [TestMethod]
        public async Task
            AppointmentsGet_ReturnsAppointmentsGetResponse_WhenValidlyRequestedWithoutPastAppointmentsFromDate()
        {
            // Arrange
            var expectedResponse = _fixture.Create<AppointmentsGetResponse>();

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Get, "patient/appointments")
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.AppointmentsGet(_odsCode, _nhsNumber, null);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task AppointmentsPost_Returns201Created_WhenValidlyRequested()
        {
            // Arrange
            var request = _fixture.Create<BookAppointmentSlotPostRequest>();
            const string expectedResponse = "Appointment successfully created.";

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Post, "patient/appointments")
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .WithContent(JsonConvert.SerializeObject(request))
                .Respond(HttpStatusCode.Created, "text/html", expectedResponse);

            // Act
            var response = await _systemUnderTest.AppointmentsPost(_odsCode, _nhsNumber, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [TestMethod]
        public async Task AppointmentsDelete_Returns204NoContent_WhenValidlyRequested()
        {
            // Arrange
            var request = _fixture.Create<CancelAppointmentDeleteRequest>();
            const string expectedResponse = "";

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Delete, "patient/appointments")
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .Respond(HttpStatusCode.NoContent, "text/html", expectedResponse);

            // Act
            var response = await _systemUnderTest.AppointmentsDelete(_odsCode, _nhsNumber, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [TestMethod]
        public async Task DemographicsGet_ReturnsDemographicsResponse_WhenValidlyRequested()
        {
            // Arrange
            var expectedResponse = _fixture.Create<DemographicsGetResponse>();

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Get, "patient/demographics")
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.DemographicsGet(_odsCode, _nhsNumber);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task DemographicsGet_ReturnsNhsUnparsableException_WhenResponseIsNotJson()
        {
            // Arrange
            var nonJsonResponse = _fixture.Create<string>();

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Get, "patient/demographics")
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .Respond(HttpStatusCode.OK, "application/json", nonJsonResponse);

            // Act
            Func<Task> act = async () => { await _systemUnderTest.DemographicsGet(_odsCode, _nhsNumber); };

            // Assert
            await act.Should().ThrowAsync<NhsUnparsableException>();
        }

        [TestMethod]
        public async Task PrescriptionHistoryGet_ReturnsPrescriptionHistoryResponse_WhenValidlyRequested()
        {
            var expectedResponse = _fixture.Create<PrescriptionHistoryGetResponse>();

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Get,
                    "patient/prescriptions")
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            var fromDate = new DateTime(2000, 1, 1);

            var response = await _systemUnderTest.PrescriptionHistoryGet(_odsCode, _nhsNumber, fromDate);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task CoursesGet_ReturnsACoursesResponse_WhenValidlyRequested()
        {
            var expectedResponse = _fixture.Create<CoursesGetResponse>();

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Get, "patient/courses")
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _systemUnderTest.CoursesGet(_odsCode, _nhsNumber);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task PrescriptionsPost_ReturnsTrue_WhenValidlyRequested()
        {
            var expectedResponse = new PrescriptionRequestPostResponse();

            var prescriptionRequestsPost = new PrescriptionRequestsPost
            {
                CourseIds = new []
                {
                    "766ecd82-3008-4454-95a5-98c423ce0527",
                }
            };

            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Post, "patient/prescriptions")
                .WithMicrotestHeaders(_odsCode, _nhsNumber)
                .WithContent(JsonConvert.SerializeObject(prescriptionRequestsPost))
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _systemUnderTest.PrescriptionsPost(_odsCode, _nhsNumber, prescriptionRequestsPost);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}