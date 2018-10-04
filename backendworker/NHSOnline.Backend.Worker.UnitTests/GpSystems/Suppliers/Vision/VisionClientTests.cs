using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.ResponseParsers;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision
{
    [TestClass]
    public sealed class VisionClientTests : IDisposable
    {
        private IVisionClient _sut;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IVisionConfig> _configMock;
        private IFixture _fixture;
        private VisionConnectionToken _connectionToken;
        private string _odsCode;
        private VisionHttpClient _httpClient;
        
        private const string RequestUserName = "username";

        private static readonly Uri ApiUrl = new Uri("http://vision_base_url/", UriKind.Absolute);
        private const string Path = "GpSystems/Suppliers/Vision/Resources/mycert.pfx";
        private const string Passphrase = "password1";
        private const string VisionTestDataDirectory = "GpSystems/Suppliers/Vision/TestData";

        private Mock<ICertificateService> _mockCertificateService;
        private Mock<IEnvelopeService> _mockEnvelopeService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IXmlResponseParser>(() => new XmlResponseParser());
            _configMock = new Mock<IVisionConfig>();
            _configMock.SetupGet(x => x.ApiUrl).Returns(ApiUrl);
            _configMock.SetupGet(x => x.RequestUsername).Returns(RequestUserName);
            _configMock.SetupGet(x => x.CertificatePath).Returns(Path);
            _configMock.SetupGet(x => x.CertificatePassphrase).Returns(Passphrase);
            
            _mockCertificateService = _fixture.Freeze<Mock<ICertificateService>>();
            _mockEnvelopeService = _fixture.Freeze<Mock<IEnvelopeService>>();
            _connectionToken = _fixture.Create<VisionConnectionToken>();
            _odsCode = _fixture.Create<string>();
            
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(VisionConstants.GetVisionRequestXml);

            _mockEnvelopeService.Setup(x => x.BuildEnvelope(It.IsAny<X509Certificate2>(), It.IsAny<VisionRequest<Object>>(), It.IsAny<string>())).Returns("AnyString");
            
            _mockCertificateService.Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>())).Returns(
                new X509Certificate2(Path, Passphrase));
            
            _mockHttpHandler = new MockHttpMessageHandler();
            _httpClient = new VisionHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            _fixture.Inject(_configMock);
            _fixture.Inject(_httpClient);
            
            _sut = _fixture.Create<VisionClient>();
        }

        [TestMethod]
        public async Task VisionRespondsWithInvalidRequest()
        {
            // Arrage
            _fixture.Customize<PatientNumber>(c => c.With(s => s.Number, "9434765919"));
            
            var xmlText = File.ReadAllText($"{VisionTestDataDirectory}/requestWasInvalidResponse.xml");
            var responseContent = new StringContent(xmlText);
            _mockHttpHandler.WhenVision(HttpMethod.Post, ApiUrl).Respond(HttpStatusCode.OK, responseContent);
            
            // Act
            var response = await _sut.GetConfiguration(_connectionToken, _odsCode);

            // Assert
            response.HasErrorResponse.Should().BeTrue();
            response.IsInvalidRequestError.Should().BeTrue();
        }

        [TestMethod]
        public async Task VisionRespondsWithInvalidSecurityHeader()
        {
            // Arrage
            _fixture.Customize<PatientNumber>(c => c.With(s => s.Number, "9434765919"));

            var xmlText = File.ReadAllText($"{VisionTestDataDirectory}/invalidSecurityHeaderResponse.xml");
            var responseContent = new StringContent(xmlText);
            _mockHttpHandler.WhenVision(HttpMethod.Post, ApiUrl).Respond(HttpStatusCode.OK, responseContent);

            // Act
            var response = await _sut.GetConfiguration(_connectionToken, _odsCode);

            // Assert
            response.HasErrorResponse.Should().BeTrue();
            response.IsInvalidSecurtyHeaderError.Should().BeTrue();
        }

        [TestMethod]
        public async Task VisionRespondsWithInvalidUserCredentials()
        {
            // Arrage
            _fixture.Customize<PatientNumber>(c => c.With(s => s.Number, "9434765919"));

            var xmlText = File.ReadAllText($"{VisionTestDataDirectory}/invalidUserCredentialsResponse.xml");
            var responseContent = new StringContent(xmlText);
            _mockHttpHandler.WhenVision(HttpMethod.Post, ApiUrl).Respond(HttpStatusCode.OK, responseContent);

            // Act
            var response = await _sut.GetConfiguration(_connectionToken, _odsCode);

            // Assert
            response.HasErrorResponse.Should().BeTrue();
            response.IsInvalidUserCredentialsError.Should().BeTrue();
        }

        [TestMethod]
        public async Task VisionRespondsWithUnknownError()
        {
            // Arrage
            _fixture.Customize<PatientNumber>(c => c.With(s => s.Number, "9434765919"));

            var xmlText = File.ReadAllText($"{VisionTestDataDirectory}/unknownErrorResponse.xml");
            var responseContent = new StringContent(xmlText);
            _mockHttpHandler.WhenVision(HttpMethod.Post, ApiUrl).Respond(HttpStatusCode.OK, responseContent);

            // Act
            var response = await _sut.GetConfiguration(_connectionToken, _odsCode);

            // Assert
            response.HasErrorResponse.Should().BeTrue();
            response.IsUnknownError.Should().BeTrue();
        }

        [TestMethod]
        public async Task GetConfigurationPostRequest_ReturnsPatientConfiguration_WhenValidRequested()
        {
            // Arrage
            _fixture.Customize<PatientNumber>(c => c.With(s => s.Number, "9434765919"));
            
            var bodyResponse  = _fixture.Create<VisionResponseEnvelope<PatientConfigurationResponse>>();

            try
            {
                var responseContent = new StringContent(bodyResponse.SerializeXml());
                _mockHttpHandler.WhenVision(HttpMethod.Post, ApiUrl)
                .Respond(HttpStatusCode.OK, responseContent);
            }
            catch(Exception e)
            {
                var ex = e;
            }
            
            // Act
            var response = await _sut.GetConfiguration(_connectionToken, _odsCode);

            // Assert
            response.Body.Should().BeEquivalentTo(bodyResponse.Body.VisionResponse.ServiceContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}
