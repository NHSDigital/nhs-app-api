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
using NHSOnline.Backend.NominatedPharmacy.Clients;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.Models;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;
using NHSOnline.Backend.NominatedPharmacy.Soap;

namespace NHSOnline.Backend.NominatedPharmacy.UnitTests
{
    [TestClass]
    public sealed class NominatedPharmacySubmitClientTests : IDisposable
    {
        private INominatedPharmacySubmitClient _sut;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<INominatedPharmacyConfig> _configMock;
        private IFixture _fixture;
        private string _odsCode;
        private NominatedPharmacyHttpClient _httpClient;
        private static readonly Uri ApiUrl = new Uri("http://spine_nominated_pharmacy_base_url/", UriKind.Absolute);
        private const string PdsPath = "syncservice-pds/pds";
        private const string UpdateNominatedPharmacySoapActionName = "urn:nhs:names:services:pds/PRPA_IN000203UK06";


        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IXmlResponseParser>(() => new XmlResponseParser());
            _configMock = _fixture.Freeze<Mock<INominatedPharmacyConfig>>();
            _configMock.SetupGet(x => x.BaseUrl).Returns(ApiUrl);

            _odsCode = _fixture.Create<string>();

            _mockHttpHandler = new MockHttpMessageHandler();
            _httpClient = new NominatedPharmacyHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            _fixture.Inject(_httpClient);

            _sut = _fixture.Create<NominatedPharmacySubmitClient>();
        }

        [TestMethod]
        public async Task UpdateNominatedPharmacy_Returns202_WhenUpdatedRequested()
        {
            // Arrange         
            var nominatedPharmacyUpdateRequest = new NominatedPharmacyUpdateRequest(
                "111", 
                "ODSFFF",
                "444");

            _mockHttpHandler
                .WhenNominatedPharmacy(HttpMethod.Post, new Uri(ApiUrl, PdsPath))
                .WithContent(nominatedPharmacyUpdateRequest.Body())
                .WithHeaders("SoapAction", UpdateNominatedPharmacySoapActionName)
                .Respond(HttpStatusCode.Accepted);

            // Act
            var response = await _sut.UpdateNominatedPharmacy(nominatedPharmacyUpdateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}