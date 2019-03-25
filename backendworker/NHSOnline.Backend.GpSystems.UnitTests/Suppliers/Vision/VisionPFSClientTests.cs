using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Certificate;
using RichardSzalay.MockHttp;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision
{
    [TestClass]
    public sealed class VisionPFSClientTests : IDisposable
    {
        private IVisionPFSClient _systemUnderTest;
        private MockHttpMessageHandler _mockHttpHandler;
        private IOptions<ConfigurationSettings> _options;
        private Mock<IVisionPFSConfig> _configMock;
        private IFixture _fixture;
        private VisionConnectionToken _connectionToken;
        private string _odsCode;
        private VisionUserSession _visionUserSession;
        private VisionPFSHttpClient _httpClient;
        private const string RequestUserName = "username";
        
        private static readonly Uri ApiUrl = new Uri("http://vision_base_url/", UriKind.Absolute);
        private const string Path = "Suppliers/Vision/Resources/mycert.pfx";
        private const string Passphrase = "password1";
        private const string VisionTestDataDirectory = "Suppliers/Vision/TestData";
        private const string GetConfigurationServiceDefinitionName = "VOS.GetConfiguration";
        private const string GetEligibleRepeatsServiceDefinitionName = "VONREP.GetEligibleRepeats";
        private const string GetHistoryServiceDefinitionName = "VONREP.GetHistory";
        private const string OrderNewPrescriptionServiceDefinitionName = "VONREP.NewPrescription";
        private const string GetAvailableAppointmentsServiceDefinitionName = "VOAPP.GetAvailableAppointments";
        private const int VisionAppointmentSlotsRequestCount = 50;
        
        private Mock<ICertificateService> _mockCertificateService;
        private Mock<IEnvelopeService> _mockEnvelopeService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _options = Options.Create(new ConfigurationSettings
            {
                VisionAppointmentSlotsRequestCount = VisionAppointmentSlotsRequestCount
            });
            _fixture.Inject(_options);
            _fixture.Register<IXmlResponseParser>(() => new XmlResponseParser());
            _configMock = _fixture.Freeze<Mock<IVisionPFSConfig>>();
            _configMock.SetupGet(x => x.ApiUrl).Returns(ApiUrl);
            _configMock.SetupGet(x => x.RequestUsername).Returns(RequestUserName);
            _configMock.SetupGet(x => x.CertificatePath).Returns(Path);
            _configMock.SetupGet(x => x.CertificatePassphrase).Returns(Passphrase);
            
            _mockCertificateService = _fixture.Freeze<Mock<ICertificateService>>();
            _mockEnvelopeService = _fixture.Freeze<Mock<IEnvelopeService>>();
            _connectionToken = _fixture.Create<VisionConnectionToken>();
            _odsCode = _fixture.Create<string>();
            _visionUserSession = _fixture.Create<VisionUserSession>();
            
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(VisionConstants.GetVisionRequestXml);

            _mockEnvelopeService.Setup(x => x.BuildEnvelope(It.IsAny<X509Certificate2>(), It.IsAny<VisionRequest<Object>>(), It.IsAny<string>())).Returns("AnyString");
            
            _mockCertificateService.Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>())).Returns(
                new X509Certificate2(Path, Passphrase));
            
            _mockHttpHandler = new MockHttpMessageHandler();
            _httpClient = new VisionPFSHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            _fixture.Inject(_httpClient);
            
            _systemUnderTest = _fixture.Create<VisionPFSClient>();
        }

        [TestMethod]
        public async Task VisionRespondsWithInvalidRequest()
        {
            // Arrange
            _fixture.Customize<PatientNumber>(c => c.With(s => s.Number, "9434765919"));
            
            var xmlText = File.ReadAllText($"{VisionTestDataDirectory}/requestWasInvalidResponse.xml");
            var responseContent = new StringContent(xmlText);
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, ApiUrl)
                .WithVisionHeaders(GetConfigurationServiceDefinitionName)
                .Respond(HttpStatusCode.OK, responseContent);
            
            // Act
            var response = await _systemUnderTest.GetConfiguration(_connectionToken, _odsCode);

            // Assert
            response.HasErrorResponse.Should().BeTrue();
            response.IsInvalidRequestError.Should().BeTrue();
        }

        [TestMethod]
        public async Task VisionRespondsWithInvalidSecurityHeader()
        {
            // Arrange
            _fixture.Customize<PatientNumber>(c => c.With(s => s.Number, "9434765919"));

            var xmlText = File.ReadAllText($"{VisionTestDataDirectory}/invalidSecurityHeaderResponse.xml");
            var responseContent = new StringContent(xmlText);
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, ApiUrl)
                .WithVisionHeaders(GetConfigurationServiceDefinitionName)
                .Respond(HttpStatusCode.OK, responseContent);

            // Act
            var response = await _systemUnderTest.GetConfiguration(_connectionToken, _odsCode);

            // Assert
            response.HasErrorResponse.Should().BeTrue();
            response.IsInvalidSecurityHeaderError.Should().BeTrue();
        }

        [TestMethod]
        public async Task VisionRespondsWithInvalidUserCredentials()
        {
            // Arrange
            _fixture.Customize<PatientNumber>(c => c.With(s => s.Number, "9434765919"));

            var xmlText = File.ReadAllText($"{VisionTestDataDirectory}/invalidUserCredentialsResponse.xml");
            var responseContent = new StringContent(xmlText);
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, ApiUrl)
                .WithVisionHeaders(GetConfigurationServiceDefinitionName)
                .Respond(HttpStatusCode.OK, responseContent);

            // Act
            var response = await _systemUnderTest.GetConfiguration(_connectionToken, _odsCode);

            // Assert
            response.HasErrorResponse.Should().BeTrue();
            response.IsInvalidUserCredentialsError.Should().BeTrue();
        }

        [TestMethod]
        public async Task VisionRespondsWithUnknownError()
        {
            // Arrange
            _fixture.Customize<PatientNumber>(c => c.With(s => s.Number, "9434765919"));

            var xmlText = File.ReadAllText($"{VisionTestDataDirectory}/unknownErrorResponse.xml");
            var responseContent = new StringContent(xmlText);
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, ApiUrl)
                .WithVisionHeaders(GetConfigurationServiceDefinitionName)
                .Respond(HttpStatusCode.OK, responseContent);

            // Act
            var response = await _systemUnderTest.GetConfiguration(_connectionToken, _odsCode);

            // Assert
            response.HasErrorResponse.Should().BeTrue();
            response.IsUnknownError.Should().BeTrue();
        }

        [TestMethod]
        public async Task GetConfigurationPostRequest_ReturnsPatientConfiguration_WhenValidRequested()
        {
            // Arrange
            _fixture.Customize<PatientNumber>(c => c.With(s => s.Number, "9434765919"));
            
            var bodyResponse  = _fixture.Create<VisionResponseEnvelope<PatientConfigurationResponse>>();
            
            var responseContent = new StringContent(bodyResponse.SerializeXml());
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, ApiUrl)
                .WithVisionHeaders(GetConfigurationServiceDefinitionName)
                .Respond(HttpStatusCode.OK, responseContent);
            
            // Act
            var response = await _systemUnderTest.GetConfiguration(_connectionToken, _odsCode);

            // Assert
            response.Body.Should().BeEquivalentTo(bodyResponse.Body.VisionResponse.ServiceContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [TestMethod]
        public async Task GetEligibleRepeatsPostRequest_ReturnsEligibleRepeats_WhenValidRequested()
        {
            // Arrange
            _fixture.Customize<PatientNumber>(c => c.With(s => s.Number, "9434765919"));
            
            var bodyResponse  = _fixture.Create<VisionResponseEnvelope<PatientConfigurationResponse>>();

            var responseContent = new StringContent(bodyResponse.SerializeXml());
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, ApiUrl)
                .WithVisionHeaders(GetConfigurationServiceDefinitionName)
                .Respond(HttpStatusCode.OK, responseContent);
            
            // Act
            var response = await _systemUnderTest.GetConfiguration(_connectionToken, _odsCode);

            // Assert
            response.Body.Should().BeEquivalentTo(bodyResponse.Body.VisionResponse.ServiceContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [TestMethod]
        public async Task GetEligibleRepeats_ReturnsEligibleRepeats_WhenValidRequested()
        {
            // Arrange
            var bodyResponse = _fixture.Create<VisionResponseEnvelope<EligibleRepeatsResponse>>(); 
            
            _mockEnvelopeService.Setup(x => x.BuildEnvelope(
                It.IsAny<X509Certificate2>(),
                It.IsAny<VisionRequest<CoursesRequest>>(), 
                It.IsAny<string>())).Returns("AnyString");

            var responseContent = new StringContent(bodyResponse.SerializeXml());
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, ApiUrl)
                .WithVisionHeaders(GetEligibleRepeatsServiceDefinitionName)
                .Respond(HttpStatusCode.OK, responseContent);

            // Act
            var response = await _systemUnderTest.GetEligibleRepeats(_visionUserSession);

            // Assert
            response.Body.Should().BeEquivalentTo(bodyResponse.Body.VisionResponse.ServiceContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [TestMethod]
        public async Task GetHistoricPrescriptions_ReturnsPrescriptionHistory_WhenValidRequested()
        {
            // Arrange
            var request = _fixture.Create<PrescriptionRequest>();

            var bodyResponse = _fixture.Create<VisionResponseEnvelope<PrescriptionHistoryResponse>>();

            _mockEnvelopeService.Setup(x => x.BuildEnvelope(
                It.IsAny<X509Certificate2>(),
                It.Is<VisionRequest<PrescriptionRequest>>(pr => pr.ServiceContent.ServiceContentBody == request),
                It.IsAny<string>())).Returns("requestXml");

            var responseContent = new StringContent(bodyResponse.SerializeXml());
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, ApiUrl)
                .WithContent("requestXml")
                .WithVisionHeaders(GetHistoryServiceDefinitionName)
                .Respond(HttpStatusCode.OK, responseContent);

            // Act
            var response = await _systemUnderTest.GetHistoricPrescriptions(_visionUserSession, request);

            // Assert
            response.Body.Should().BeEquivalentTo(bodyResponse.Body.VisionResponse.ServiceContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task OrderNewPrescription_ReturnsSuccessfulResponse_WhenValidRequested()
        {
            // Arrange
            var request = _fixture.Create<OrderNewPrescriptionRequest>();

            var bodyResponse = _fixture.Create<VisionResponseEnvelope<OrderNewPrescriptionResponse>>();

            _mockEnvelopeService.Setup(x => x.BuildEnvelope(
                It.IsAny<X509Certificate2>(),
                It.Is<VisionRequest<OrderNewPrescriptionRequest>>(pr => pr.ServiceContent.ServiceContentBody == request),
                It.IsAny<string>())).Returns("requestXml");

            var responseContent = new StringContent(bodyResponse.SerializeXml());
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, ApiUrl)
                .WithContent("requestXml")
                .WithVisionHeaders(OrderNewPrescriptionServiceDefinitionName)
                .Respond(HttpStatusCode.OK, responseContent);

            // Act
            var response = await _systemUnderTest.OrderNewPrescription(_visionUserSession, request);

            // Assert
            response.Body.Should().BeEquivalentTo(bodyResponse.Body.VisionResponse.ServiceContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetAvailableAppointments_FormatsRequestCorrectly()
        {
            // Arrange
            var dateRange = new AppointmentSlotsDateRange(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(29));
            var bodyResponse = _fixture.Create<VisionResponseEnvelope<AvailableAppointmentsResponse>>();
            var received = new VisionRequest<AvailableAppointmentsRequest>();

            _visionUserSession.LocationIds = _fixture.CreateMany<string>().ToList();

            var slotsRequest = GetSlotsRequest(dateRange);

            var expectedVisionRequest = GetVisionRequest(slotsRequest);

            _mockEnvelopeService.Reset();

            _mockEnvelopeService.Setup(x => x.BuildEnvelope(
                It.IsAny<X509Certificate2>(),
                It.IsAny<VisionRequest<AvailableAppointmentsRequest>>(),
                It.IsAny<string>()))
                .Returns("AnyString")
                .Callback<X509Certificate, VisionRequest<AvailableAppointmentsRequest>, string>((c, r, s) =>
                    received = r);

            var responseContent = new StringContent(bodyResponse.SerializeXml());
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, ApiUrl)
                .WithVisionHeaders(GetAvailableAppointmentsServiceDefinitionName)
                .Respond(HttpStatusCode.OK, responseContent);

            // Act
            await _systemUnderTest.GetAvailableAppointments(_visionUserSession, dateRange);

            // Assert
            received.ServiceContent.Should().BeEquivalentTo(expectedVisionRequest.ServiceContent);
        }

        private VisionRequest<AvailableAppointmentsRequest> GetVisionRequest(AvailableAppointmentsRequest slotRequest)
        {
            return new VisionRequest<AvailableAppointmentsRequest>("VOAPP.GetAvailableAppointments",
                "2.0.0",
                _visionUserSession.RosuAccountId,
                _visionUserSession.ApiKey,
                _visionUserSession.OdsCode,
                null,
                slotRequest);
        }

        private AvailableAppointmentsRequest GetSlotsRequest(AppointmentSlotsDateRange dateRange)
        {
            return new AvailableAppointmentsRequest
            {
                PatientId = _visionUserSession.PatientId,
                Page = new Page
                {
                    Number = 1,
                    SlotsPerPage = VisionAppointmentSlotsRequestCount
                },
                Locations = _visionUserSession.LocationIds,
                Owners = new List<string> { "ALL" },
                DateRange = new DateRange
                {
                    From = dateRange.FromDate.Date,
                    To = dateRange.ToDate.Date.AddDays(-1)
                }
            };
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}
