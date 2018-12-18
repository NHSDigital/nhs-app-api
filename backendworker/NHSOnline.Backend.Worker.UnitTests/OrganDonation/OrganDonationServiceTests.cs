using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationServiceTests
    {
        private OrganDonationService _organDonationService;
        private Mock<IOrganDonationClient> _mockOrganDonationClient;

        private Mock<IMapper<OrganDonationRegistration, LookupRegistrationRequest>>
            _mockLookupRegistrationRequestMapper;

        private IFixture _fixture;
        private UserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockOrganDonationClient = _fixture.Freeze<Mock<IOrganDonationClient>>();
            _mockLookupRegistrationRequestMapper =
                _fixture.Freeze<Mock<IMapper<OrganDonationRegistration, LookupRegistrationRequest>>>();

            _userSession = _fixture.Create<UserSession>();

            _organDonationService = _fixture.Create<OrganDonationService>();
        }

        [TestMethod]
        public void GetRegistration_WhenCalledAndNoExistingRegistration_ReturnsNewRegistrationResponse()
        {
            // Arrange 
            var demographicsResponse = new DemographicsResponse();
            var demographicsResult = new DemographicsResult.SuccessfullyRetrieved(demographicsResponse);
            var organDonationResponse = new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.NotFound);

            _mockOrganDonationClient.Setup(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult(organDonationResponse));

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.NewRegistration>();
        }

        [TestMethod]
        public void GetRegistration_WhenCalledAndWithExistingRegistration_ReturnsExistingRegistrationResponse()
        {
            // Arrange 
            var demographicsResponse = new DemographicsResponse();
            var demographicsResult = new DemographicsResult.SuccessfullyRetrieved(demographicsResponse);
            var organDonationResponse = new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.OK);

            _mockOrganDonationClient.Setup(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult(organDonationResponse));

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.ExistingRegistration>();
        }

        [TestMethod]
        public void GetRegistration_WhenCalledAndSearchFailsWithException_ReturnsSearchSystemUnavailableResponse()
        {
            // Arrange 
            var demographicsResponse = new DemographicsResponse();
            var demographicsResult = new DemographicsResult.SuccessfullyRetrieved(demographicsResponse);

            _mockOrganDonationClient.Setup(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession))
                .Throws<HttpRequestException>();

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.SearchSystemUnavailable>();
        }


        [TestMethod]
        public void GetRegistration_WhenCalledAndSearchFailsWithInternalError_ReturnsSearchErrorResponse()
        {
            // Arrange 
            var demographicsResponse = new DemographicsResponse();
            var demographicsResult = new DemographicsResult.SuccessfullyRetrieved(demographicsResponse);
            var organDonationResponse =
                new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.InternalServerError);

            _mockOrganDonationClient.Setup(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult(organDonationResponse));

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.SearchError>();
        }

        [TestMethod]
        public void GetRegistration_WhenCalledAndSearchFailsWithBadRequest_ReturnsBadSearchRequestResponse()
        {
            // Arrange 
            var demographicsResponse = new DemographicsResponse();
            var demographicsResult = new DemographicsResult.SuccessfullyRetrieved(demographicsResponse);
            var organDonationResponse =
                new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.BadRequest);

            _mockLookupRegistrationRequestMapper.Setup(x => x.Map(It.IsAny<OrganDonationRegistration>()))
                .Returns(new LookupRegistrationRequest());
            _mockOrganDonationClient.Setup(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult(organDonationResponse));

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.BadSearchRequest>();
        }

        [TestMethod]
        public void GetRegistration_WhenCalledAndSearchFailsWithConflict_ReturnsDuplicateResponse()
        {
            // Arrange 
            var demographicsResponse = new DemographicsResponse();
            var demographicsResult = new DemographicsResult.SuccessfullyRetrieved(demographicsResponse);
            var organDonationResponse = new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.Conflict);

            _mockOrganDonationClient.Setup(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult(organDonationResponse));

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.DuplicateRecord>();
        }

        [TestMethod]
        public void GetRegistration_WhenCalledAndSearchTimeouts_ReturnsSearchTimeoutResponse()
        {
            // Arrange 
            var demographicsResponse = new DemographicsResponse();
            var demographicsResult = new DemographicsResult.SuccessfullyRetrieved(demographicsResponse);
            var organDonationResponse =
                new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.RequestTimeout);

            _mockOrganDonationClient.Setup(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession))
                .Returns(Task.FromResult(organDonationResponse));

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<LookupRegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.SearchTimeout>();
        }

        [TestMethod]
        public void
            GetRegistration_WhenCalledAndDemographicstRetrievedSuccessfully_ReturnsDemographicsRetrievalFailedResponse()
        {
            // Arrange 
            var demographicsResult = new DemographicsResult.Unsuccessful();

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.VerifyNoOtherCalls();
            result.Result.Should().BeOfType<OrganDonationResult.DemographicsRetrievalFailed>();
        }
    }
}