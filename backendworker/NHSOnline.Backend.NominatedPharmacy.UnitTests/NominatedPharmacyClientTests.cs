using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.ResponseParsers;
using RichardSzalay.MockHttp;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions;
using NHSOnline.Backend.NominatedPharmacy.ApiModels;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;
using NHSOnline.Backend.NominatedPharmacy.Soap;

namespace NHSOnline.Backend.NominatedPharmacy.UnitTests
{
    [TestClass]
    public sealed class NominatedPharmacyClientTests : IDisposable
    {
        private INominatedPharmacyClient _sut;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<INominatedPharmacyConfig> _configMock;
        private IFixture _fixture;
        private string _odsCode;
        private NominatedPharmacyHttpClient _httpClient;        
        private static readonly Uri ApiUrl = new Uri("http://spine_nominated_pharmacy_base_url/", UriKind.Absolute);
        private const string GetNominatedPharmacySoapActionName = "urn:nhs:names:services:pdsquery/QUPA_IN000008UK02";
        
        private Mock<IEnvelopeService> _mockEnvelopeService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IXmlResponseParser>(() => new XmlResponseParser());
            _configMock = _fixture.Freeze<Mock<INominatedPharmacyConfig>>();
            _configMock.SetupGet(x => x.BaseUrl).Returns(ApiUrl);
            
            _mockEnvelopeService = _fixture.Freeze<Mock<IEnvelopeService>>();
            _odsCode = _fixture.Create<string>();
            
            _mockHttpHandler = new MockHttpMessageHandler();
            _httpClient = new NominatedPharmacyHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            _fixture.Inject(_httpClient);
            
            _sut = _fixture.Create<NominatedPharmacyClient>();
        }
        
        [TestMethod]
        public async Task GetNominatedPharmacy_ReturnsNominatedPharmacy_WhenValidRequested()
        {
            // Arrange
            var request = new QUPA_IN000008UK02();

            var bodyResponse = new NominatedPharmacyResponseEnvelope<QUPA_IN000009UK03_Response>
            {
                Body = new Body<QUPA_IN000009UK03_Response>
                {
                    RetrievalQueryResponse = new QUPA_IN000009UK03_Response
                    {
                        QUPA_IN000009UK03 = new QUPA_IN000009UK03(),
                    }
                }
            };

            _mockEnvelopeService
                .Setup(x => x.BuildEnvelope(
                    It.Is<PdsRequest<QUPA_IN000008UK02>>(pr => pr.Body == request),
                    It.Is<IServiceDefinition>(sd => sd.SoapActionName.Equals(GetNominatedPharmacySoapActionName, StringComparison.Ordinal))))
                .Returns("requestXml")
                .Verifiable();
            
            var responseContent = new StringContent(bodyResponse.SerializeXml());

            _mockHttpHandler
                .WhenNominatedPharmacy(HttpMethod.Post, new Uri(ApiUrl, "syncservice-pds/pds"))
                .WithContent("requestXml")
                .WithHeaders("SoapAction", GetNominatedPharmacySoapActionName)
                .Respond(HttpStatusCode.OK, responseContent);
            
            // Act
            var response = await _sut.NominatedPharmacyGet(request);

            // Assert
            response.Body.QUPA_IN000009UK03.Should().BeEquivalentTo(bodyResponse.Body.RetrievalQueryResponse.QUPA_IN000009UK03);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            _mockEnvelopeService.Verify();
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}