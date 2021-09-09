using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.ResponseParsers;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision
{
    [TestClass]
    public sealed class VisionDirectServicesLinkageClientTests : IDisposable
    {
        private IVisionDirectServicesClient _systemUnderTest;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IVisionDirectServicesConfig> _visionDirectServiceConfigMock;
        private VisionConfigurationSettings _visionConfiguration;

        private VisionConnectionToken _connectionToken;
        private VisionUserSession _visionUserSession;
        private string _odsCode;
        private XmlSerializerNamespaces _xmlNamespaces;
        private IFixture _fixture;
        private VisionDirectServicesHttpClient _httpClient;

        private const string ApplicationProviderId = "TestApplicationProviderId";
        private static readonly Uri ApiUri = new Uri("http://vision_base_url/", UriKind.Absolute);
        private const string ConfigurationBasePath = "v1/organisations/{0}/onlineservices/configuration";
        private const string DemographicsBasePath = "v1/organisations/{0}/onlineservices/demographics";
        private const string PrescriptionHistoryBasePath = "v1/organisations/{0}/onlineservices/history";
        private const string VisionTestData_DirectServices_Directory = "Suppliers/Vision/TestData/DirectServices";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _connectionToken = _fixture.Create<VisionConnectionToken>();
            _odsCode = _fixture.Create<string>();

            _visionUserSession = _fixture.Create<VisionUserSession>();

            _fixture.Register<IXmlResponseParser>(() => new XmlResponseParser());

            _mockHttpHandler = new MockHttpMessageHandler();

            _visionDirectServiceConfigMock = new Mock<IVisionDirectServicesConfig>();
            _visionDirectServiceConfigMock.SetupGet(x => x.ApiUrl).Returns(ApiUri);

            _visionConfiguration =
                new VisionConfigurationSettings(ApplicationProviderId, null, "", "", "", "", "", "", "", 0, 0, 0);
            _httpClient = new VisionDirectServicesHttpClient(new HttpClient(_mockHttpHandler), _visionDirectServiceConfigMock.Object);

            _fixture.Inject(_visionDirectServiceConfigMock);
            _fixture.Inject(_httpClient);
            _fixture.Inject(_visionConfiguration);

            _xmlNamespaces = new XmlSerializerNamespaces();
            _xmlNamespaces.Add("vision", "urn:vision");

            _systemUnderTest = _fixture.Create<VisionDirectServicesClient>();
        }

        [TestMethod]
        public async Task GetConfiguration_ReturnsConfigurationResponse_WhenRequested()
        {
            var expectedRequestContent = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><vision:visionRequest xmlns:vision=\"urn:vision\"><vision:credentials><vision:rosuAccountId>{ _connectionToken.RosuAccountId }</vision:rosuAccountId><vision:apiKey>{ _connectionToken.ApiKey }</vision:apiKey></vision:credentials><vision:opsReference><vision:provider>{ ApplicationProviderId }</vision:provider><vision:accountId>{ ApplicationProviderId }</vision:accountId></vision:opsReference></vision:visionRequest>";
            var expectedUri = new Uri(
                ApiUri,
                string.Format(CultureInfo.CurrentCulture, ConfigurationBasePath, _odsCode));

            var xmlResponseText = await File.ReadAllTextAsync($"{VisionTestData_DirectServices_Directory}/validGetConfigurationResponse.xml");
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, expectedUri)
                .WithContent(expectedRequestContent)
                .Respond("application/xml", xmlResponseText);

            var response = await _systemUnderTest.GetConfigurationV2(_connectionToken, _odsCode);

            response.Body.Configuration.Account.Name.Should().Be("Mrs TestNameFirst TestNameSecond");
            response.StatusCode.Should().Be(200);
            response.HasSuccessResponse.Should().BeTrue();
        }

        [TestMethod]
        public async Task GetDemographics_ReturnsValidResponse_WhenRequested()
        {
            var demographicsRequest = new DemographicsRequest();
            var expectedRequestContent = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><vision:visionRequest xmlns:vision=\"urn:vision\"><vision:credentials><vision:rosuAccountId>{ _visionUserSession.RosuAccountId }</vision:rosuAccountId><vision:apiKey>{ _visionUserSession.ApiKey }</vision:apiKey></vision:credentials><vision:opsReference><vision:provider>{ ApplicationProviderId }</vision:provider><vision:accountId>{ ApplicationProviderId }</vision:accountId></vision:opsReference><vision:vos><vision:patientId>{ _visionUserSession.PatientId }</vision:patientId></vision:vos></vision:visionRequest>";
            var expectedUri = new Uri(
                ApiUri,
                string.Format(CultureInfo.CurrentCulture, DemographicsBasePath, _visionUserSession.OdsCode));

            var xmlResponseText = await File.ReadAllTextAsync($"{VisionTestData_DirectServices_Directory}/validGetDemographicsResponse.xml");
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, expectedUri)
                .WithContent(expectedRequestContent)
                .Respond("application/xml", xmlResponseText);

            var response = await _systemUnderTest.GetDemographicsV2(_visionUserSession, demographicsRequest);

            response.Body.Demographics.Name.Title.Should().Be("MS");
            response.Body.Demographics.Name.Forename.Should().Be("TESTFORENAME");
            response.Body.Demographics.Name.Surname.Should().Be("TESTSURNAME");
            response.Body.Demographics.MaritalStatus.Text.Should().Be("Unknown");
            response.Body.Demographics.Gender.Text.Should().Be("Female");
            response.Body.Demographics.PrimaryAddress.County.Should().Be("ANTRIM");
            response.Body.Demographics.PrimaryAddress.Postcode.Should().Be("BT7 1NT");
            response.Body.Demographics.PrimaryAddress.Street.Should().Be("UPPER CRESCENT");
            response.Body.Demographics.PrimaryAddress.Town.Should().Be("TEST2");
            response.Body.Demographics.PrimaryAddress.HouseName.Should().Be("HOUSE");
            response.Body.Demographics.PrimaryAddress.HouseNumber.Should().Be("23");
            response.Body.Demographics.DateOfBirth.Day.Should().Be(21);
            response.Body.Demographics.DateOfBirth.Month.Should().Be(9);
            response.Body.Demographics.DateOfBirth.Year.Should().Be(1970);
            response.StatusCode.Should().Be(200);
            response.HasSuccessResponse.Should().BeTrue();
        }

        [TestMethod]
        public async Task GetPrescriptionHistory_ReturnsValidResponse_WhenRequested()
        {
            var prescriptionRequest = new PrescriptionRequest();
            var expectedRequestContent = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><vision:visionRequest xmlns:vision=\"urn:vision\"><vision:credentials><vision:rosuAccountId>{ _visionUserSession.RosuAccountId }</vision:rosuAccountId><vision:apiKey>{ _visionUserSession.ApiKey }</vision:apiKey></vision:credentials><vision:opsReference><vision:provider>{ ApplicationProviderId }</vision:provider><vision:accountId>{ ApplicationProviderId }</vision:accountId></vision:opsReference><vision:vos><vision:patientId>{ _visionUserSession.PatientId }</vision:patientId></vision:vos></vision:visionRequest>";
            var expectedUri = new Uri(
                ApiUri,
                string.Format(CultureInfo.CurrentCulture, PrescriptionHistoryBasePath, _visionUserSession.OdsCode));

            var xmlResponseText = await File.ReadAllTextAsync($"{VisionTestData_DirectServices_Directory}/validGetPrescriptionHistoryResponse.xml");
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, expectedUri)
                .WithContent(expectedRequestContent)
                .Respond("application/xml", xmlResponseText);

            var response = await _systemUnderTest.GetPrescriptionHistoryV2(_visionUserSession, prescriptionRequest);

            response.Body.PrescriptionHistory.Requests[2].Date.Day.Should().Be(19);
            response.Body.PrescriptionHistory.Requests[2].Date.Month.Should().Be(3);
            response.Body.PrescriptionHistory.Requests[2].Date.Year.Should().Be(2021);
            response.Body.PrescriptionHistory.Requests[2].Status.Code.Should().Be(-2);
            response.Body.PrescriptionHistory.Requests[2].Status.Text.Should().Be("Not Processed");
            response.Body.PrescriptionHistory.Requests[2].Repeats[0].Drug.Should().Be("Paracetamol 500mg capsules");
            response.Body.PrescriptionHistory.Requests[2].Repeats[0].Dosage.Should().Be("1 TO 2 CAPSULES UP TO FOUR TIMES DAILY AS REQUIRED");
            response.Body.PrescriptionHistory.Requests[2].Repeats[0].Quantity.Should().Be("(21) capsule");
            response.StatusCode.Should().Be(200);
            response.HasSuccessResponse.Should().BeTrue();
        }

        [TestMethod]
        public async Task VisionRespondsWithUnknownError()
        {
            var expectedRequestContent = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><vision:visionRequest xmlns:vision=\"urn:vision\"><vision:credentials><vision:rosuAccountId>{ _connectionToken.RosuAccountId }</vision:rosuAccountId><vision:apiKey>{ _connectionToken.ApiKey }</vision:apiKey></vision:credentials><vision:opsReference><vision:provider>{ ApplicationProviderId }</vision:provider><vision:accountId>{ ApplicationProviderId }</vision:accountId></vision:opsReference></vision:visionRequest>";
            var expectedUri = new Uri(
                ApiUri,
                string.Format(CultureInfo.CurrentCulture, ConfigurationBasePath, _odsCode));

            var xmlText = await File.ReadAllTextAsync($"{VisionTestData_DirectServices_Directory}/unknownErrorResponse.xml");
            var responseContent = new StringContent(xmlText);
            _mockHttpHandler
                .WhenVision(HttpMethod.Post, expectedUri)
                .WithContent(expectedRequestContent)
                .Respond(HttpStatusCode.BadRequest, responseContent);

            var response = await _systemUnderTest.GetConfigurationV2(_connectionToken, _odsCode);

            response.StatusCode.Should().Be(400);
            response.HasErrorResponse.Should().BeTrue();
            response.ErrorText.Should().Be("Unknown Error");
            response.ErrorDescription.Should().Be("ERROR: invalid input syntax for integer: \"\"");
        }

        [TestCleanup]
        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}