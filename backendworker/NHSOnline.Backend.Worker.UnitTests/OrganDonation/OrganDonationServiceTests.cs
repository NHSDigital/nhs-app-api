using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationServiceTests
    {
        private const string ReferenceDataCacheKey = "_organDonationReferenceData";
        private const int ReferenceDataExpiryHours = 6;
        private OrganDonationService _organDonationService;
        private UserSession _userSession;
        private Mock<IMemoryCache> _mockMemoryCache;
        private Mock<IOrganDonationClient> _mockOrganDonationClient;
        private Mock<IOrganDonationConfig> _mockOrganDonationConfig;
        private Mock<IMapper<OrganDonationRegistration, RegistrationLookupRequest>>
            _mockRegistrationLookupRequestMapper;
        private Mock<IMapper<OrganDonationRegistrationRequest, RegistrationRequest>>
            _mockRegistrationRequestMapper;
        
        // needed for Callback
        private delegate void TryGetValueCallback(object key, out object value);

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockMemoryCache = fixture.Freeze<Mock<IMemoryCache>>();
            _mockOrganDonationClient = fixture.Freeze<Mock<IOrganDonationClient>>();
            _mockOrganDonationConfig = fixture.Freeze<Mock<IOrganDonationConfig>>();
            _mockRegistrationLookupRequestMapper =
                fixture.Freeze<Mock<IMapper<OrganDonationRegistration, RegistrationLookupRequest>>>();
            
            _mockRegistrationRequestMapper =
                fixture.Freeze<Mock<IMapper<OrganDonationRegistrationRequest, RegistrationRequest>>>();

            _userSession = fixture.Create<UserSession>();
            _organDonationService = fixture.Create<OrganDonationService>();
        }

        [TestMethod]
        public void GetOrganDonation_WhenCalledAndNoExistingRegistration_ReturnsNewRegistrationResponse()
        {
            // Arrange 
            var context = SetupGetOrganDonationTest(httpStatus: HttpStatusCode.NotFound);
            
            // Act
            var result = _organDonationService.GetOrganDonation(context.DemographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.NewRegistration>();
        }

        [TestMethod]
        public void GetOrganDonation_WhenCalledAndWithExistingRegistration_ReturnsExistingRegistrationResponse()
        {
            // Arrange 
            var context = SetupGetOrganDonationTest();
                
            // Act
            var result = _organDonationService.GetOrganDonation(context.DemographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.ExistingRegistration>();
        }


        [TestMethod]
        public void GetOrganDonation_WhenCalledAndWithExistingRegistrationInConflict_ReturnsExistingRegistrationResponseInConflict()
        {
            // Arrange 
            var context = SetupGetOrganDonationTest(httpStatus: HttpStatusCode.Conflict);

            // Act
            var result = _organDonationService.GetOrganDonation(context.DemographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), _userSession));

            var registration = result.Result.Should().BeOfType<OrganDonationResult.ExistingRegistration>().Subject;

            registration.Registration.State.Should().BeEquivalentTo(State.Conflicted);
            registration.Registration.Decision.Should().BeEquivalentTo(Decision.Unknown);
        }

        [TestMethod]
        public void GetOrganDonation_WhenCalledAndSearchFailsWithException_ReturnsSearchSystemUnavailableResponse()
        {
            // Arrange 
            var context = SetupGetOrganDonationTest(true);

            // Act
            var result = _organDonationService.GetOrganDonation(context.DemographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.SearchSystemUnavailable>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.MethodNotAllowed)]
        [DataRow(HttpStatusCode.UnsupportedMediaType)]
        [DataRow(HttpStatusCode.UnprocessableEntity)]
        [DataRow(HttpStatusCode.TooManyRequests)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        public void GetOrganDonation_WhenCalledAndSearchFailsWithError_ReturnsSearchErrorResponse(HttpStatusCode httpStatus)
        {
            // Arrange 
            var context = SetupGetOrganDonationTest(httpStatus: httpStatus);

            // Act
            var result = _organDonationService.GetOrganDonation(context.DemographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.SearchError>();
        }
        
        [TestMethod]
        public void GetOrganDonation_WhenCalledAndSearchFailsWithBadRequest_ReturnsBadSearchRequestResponse()
        {
            // Arrange 
            var context = SetupGetOrganDonationTest(httpStatus: HttpStatusCode.BadRequest);

            // Act
            var result = _organDonationService.GetOrganDonation(context.DemographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.BadSearchRequest>();
        }

        [TestMethod]
        public void GetOrganDonation_WhenCalledAndSearchTimeouts_ReturnsSearchTimeoutResponse()
        {
            // Arrange 
            var context = SetupGetOrganDonationTest(httpStatus: HttpStatusCode.RequestTimeout);

            // Act
            var result = _organDonationService.GetOrganDonation(context.DemographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.SearchTimeout>();
        }

        [TestMethod]
        public void
            GetOrganDonation_WhenCalledAndDemographicstRetrievedUnsuccessfully_ReturnsDemographicsBadGateway()
        {
            // Arrange 
            var demographicsResult = new DemographicsResult.Unsuccessful();

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.VerifyNoOtherCalls();
            result.Result.Should().BeOfType<OrganDonationResult.DemographicsBadGateway>();
        }

        [TestMethod]
        public void
            GetOrganDonation_WhenCalledAndDemographicsUserHasNoAccess_ReturnsDemographicsForbidden()
        {
            // Arrange 
            var demographicsResult = new DemographicsResult.UserHasNoAccess();

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.VerifyNoOtherCalls();
            result.Result.Should().BeOfType<OrganDonationResult.DemographicsForbidden>();
        }

        [TestMethod]
        public void
            GetOrganDonation_WhenCalledAndDemographicsInternalServerError_ReturnsDemographicsInternalServerError()
        {
            // Arrange 
            var demographicsResult = new DemographicsResult.InternalServerError();

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.VerifyNoOtherCalls();
            result.Result.Should().BeOfType<OrganDonationResult.DemographicsInternalServerError>();
        }

        [TestMethod]
        public void
            GetOrganDonation_WhenCalledAndDemographicsSupplierSystemUnavailable_ReturnsDemographicsRetrievalFailed()
        {
            // Arrange 
            var demographicsResult = new DemographicsResult.SupplierSystemUnavailable();

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.VerifyNoOtherCalls();
            result.Result.Should().BeOfType<OrganDonationResult.DemographicsRetrievalFailed>();
        }

        [TestMethod]
        public void GetReferenceData_WhenCalledWithoutValueInTheCache_CacheAndReturnSuccessfulRetrieveResponse()
        {
            // Arrange
            var context = SetupGetReferenceDataTest();
            
            // Act
            var result = _organDonationService.GetReferenceData();
            
            // Assert
            _mockMemoryCache.Verify(x => x.CreateEntry(ReferenceDataCacheKey));
            _mockOrganDonationClient.Verify(x => x.GetAllReferenceData());
            context.AbsoluteExpiration.Should().BeCloseTo(DateTimeOffset.UtcNow.AddHours(ReferenceDataExpiryHours), TimeSpan.FromSeconds(1));
            result.Result.Should().BeEquivalentTo(context.Cached);
            result.Result.Should().BeOfType<OrganDonationReferenceDataResult.SuccessfullyRetrieved>();
        }
        
        [TestMethod]
        public void GetReferenceData_WhenCalledWithExistingCacheValue_ReturnSuccessfulRetrieveResponse()
        {
            // Arrange
            var organDonationReferenceDataResult =
                new OrganDonationReferenceDataResult.SuccessfullyRetrieved(new OrganDonationReferenceDataResponse());
            var context = SetupGetReferenceDataTest(cached: organDonationReferenceDataResult);

            // Act
            var result = _organDonationService.GetReferenceData();

            // Assert
            _mockOrganDonationClient.VerifyNoOtherCalls();
            result.Result.Should().BeEquivalentTo(context.Cached);
        }
        
        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.MethodNotAllowed)]
        [DataRow(HttpStatusCode.Conflict)]
        [DataRow(HttpStatusCode.UnsupportedMediaType)]
        [DataRow(HttpStatusCode.UnprocessableEntity)]
        [DataRow(HttpStatusCode.TooManyRequests)]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        [DataRow(HttpStatusCode.GatewayTimeout)]
        public void
            GetReferenceData_WhenCalledAndRetrievalFailsWithError_ReturnSystemErrorResponse(HttpStatusCode httpStatus)
        {
            // Arrange
            var context = SetupGetReferenceDataTest(httpStatus: httpStatus);

            // Act
            var result = _organDonationService.GetReferenceData();

            // Assert
            _mockOrganDonationClient.Verify(x => x.GetAllReferenceData());
            context.AbsoluteExpiration.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
            result.Result.Should().BeEquivalentTo(context.Cached);
            result.Result.Should().BeOfType<OrganDonationReferenceDataResult.UpstreamError>();
        }
        
        [TestMethod]
        public void
            GetReferenceData_WhenCalledAndRetrievalFailsWithTimeout_ReturnTimeoutResponse()
        {
            // Arrange
            var context = SetupGetReferenceDataTest(httpStatus: HttpStatusCode.RequestTimeout);

            // Act
            var result = _organDonationService.GetReferenceData();

            // Assert
            _mockOrganDonationClient.Verify(x => x.GetAllReferenceData());
            context.AbsoluteExpiration.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
            result.Result.Should().BeEquivalentTo(context.Cached);
            result.Result.Should().BeOfType<OrganDonationReferenceDataResult.Timeout>();
        }
        
        [TestMethod]
        public void
            GetReferenceData_WhenCalledAndRetrievalFailsWithException_ReturnSystemUnavaivableResponseWithNoCaching()
        {
            // Arrange
            var context = SetupGetReferenceDataTest(true);

            // Act
            var result = _organDonationService.GetReferenceData();

            // Assert
            _mockOrganDonationClient.Verify(x => x.GetAllReferenceData());
            context.Cached.Should().BeNull();
            context.AbsoluteExpiration.Should().BeNull();
            result.Result.Should().BeOfType<OrganDonationReferenceDataResult.SystemError>();
        }
        
        [TestMethod]
        public void Register_WhenCalledWithRequest_ReturnsSuccessfullyRegisteredResponse()
        {
            // Arrange 
            var context = SetupRegistrationTest();
            
            // Act
            var result = _organDonationService.Register(context.RegistrationRequest, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostRegistration(It.IsAny<RegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationRegistrationResult.SuccessfullyRegistered>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.MethodNotAllowed)]
        [DataRow(HttpStatusCode.UnsupportedMediaType)]
        [DataRow(HttpStatusCode.UnprocessableEntity)]
        [DataRow(HttpStatusCode.TooManyRequests)]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        [DataRow(HttpStatusCode.GatewayTimeout)]
        public void Register_WhenCalledAndRetrievalFailsWithError_ReturnSystemErrorResponse(HttpStatusCode httpStatus)
        {
            // Arrange
            var context = SetupRegistrationTest(httpStatus: httpStatus);

            // Act
            var result = _organDonationService.Register(context.RegistrationRequest, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostRegistration(It.IsAny<RegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationRegistrationResult.UpstreamError>();
        }
        
        [TestMethod]
        public void Register_WhenCalledAndRetrievalFailsWithTimeout_ReturnTimeoutResponse()
        {
            // Arrange
            var context = SetupRegistrationTest(httpStatus: HttpStatusCode.RequestTimeout);

            // Act
            var result = _organDonationService.Register(context.RegistrationRequest, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostRegistration(It.IsAny<RegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationRegistrationResult.Timeout>();
        }
        
        [TestMethod]
        public void Register_WhenCalledAndRetrievalFailsWithException_ReturnSystemUnavaivableResponseWithNoCaching()
        {
            // Arrange
            var context = SetupRegistrationTest(true);

            // Act
            var result = _organDonationService.Register(context.RegistrationRequest, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostRegistration(It.IsAny<RegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationRegistrationResult.SystemError>();
        }
        
        private GetReferenceDataTestContext SetupGetReferenceDataTest(
            bool throwException = false, 
            HttpStatusCode httpStatus = HttpStatusCode.OK,
            OrganDonationReferenceDataResult cached = null
        )
        {
            return new GetReferenceDataTestContext(_mockOrganDonationConfig, _mockOrganDonationClient, _mockMemoryCache, throwException, httpStatus, cached);
        }

        private GetOrganDonationTestContext SetupGetOrganDonationTest(
            bool throwException = false, 
            HttpStatusCode httpStatus = HttpStatusCode.OK
        )
        {
            return new GetOrganDonationTestContext(_mockOrganDonationClient, _userSession, throwException, httpStatus, _mockRegistrationLookupRequestMapper);
        }
        
        private RegistrationTestContext SetupRegistrationTest(
            bool throwException = false, 
            HttpStatusCode httpStatus = HttpStatusCode.OK
        )
        {
            return new RegistrationTestContext(_mockOrganDonationClient, _userSession, throwException, httpStatus, _mockRegistrationRequestMapper);
        }

        private class GetOrganDonationTestContext
        {
            public GetOrganDonationTestContext(
                Mock<IOrganDonationClient> mockClient, 
                UserSession userSession, 
                bool throwException, 
                HttpStatusCode httpStatus,
                Mock<IMapper<OrganDonationRegistration, 
                    RegistrationLookupRequest>> mockRegistrationLookupRequestMapper
            )
            {
                var demographicsResponse = new DemographicsResponse();
                var organDonationResponse = new OrganDonationResponse<RegistrationLookupResponse>(httpStatus);
                
                DemographicsResult = new DemographicsResult.SuccessfullyRetrieved(demographicsResponse);

                if (throwException)
                {
                    mockClient.Setup(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), userSession))
                        .Throws<HttpRequestException>();    
                }
                else
                {
                    mockClient.Setup(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), userSession))
                        .Returns(Task.FromResult(organDonationResponse));    
                }              
                
                mockRegistrationLookupRequestMapper
                    .Setup(x => x.Map(It.IsAny<OrganDonationRegistration>()))
                    .Returns(new RegistrationLookupRequest());
            }

            public DemographicsResult DemographicsResult { get; }
        }

        private class RegistrationTestContext
        {
            public RegistrationTestContext(
                Mock<IOrganDonationClient> mockClient,
                UserSession userSession,
                bool throwException,
                HttpStatusCode httpStatus,
                Mock<IMapper<OrganDonationRegistrationRequest,
                RegistrationRequest>> mockRegistrationRequestMapper
            )
            {
                RegistrationRequest = new OrganDonationRegistrationRequest();
                var organDonationResponse = new OrganDonationResponse<RegistrationResponse>(httpStatus);

                if (throwException)
                {
                    mockClient.Setup(x => x.PostRegistration(It.IsAny<RegistrationRequest>(), userSession))
                        .Throws<HttpRequestException>();
                }
                else
                {
                    mockClient.Setup(x => x.PostRegistration(It.IsAny<RegistrationRequest>(), userSession))
                        .Returns(Task.FromResult(organDonationResponse));
                }

                mockRegistrationRequestMapper
                    .Setup(x => x.Map(It.IsAny<OrganDonationRegistrationRequest>()))
                    .Returns(new RegistrationRequest());
            }

            public OrganDonationRegistrationRequest RegistrationRequest { get; }
        }
        
        private class GetReferenceDataTestContext
        {
            public GetReferenceDataTestContext(
                Mock<IOrganDonationConfig> mockConfig,
                Mock<IOrganDonationClient> mockClient, 
                Mock<IMemoryCache> mockCache,
                bool throwException, 
                HttpStatusCode httpStatus,
                OrganDonationReferenceDataResult cached
            )
            {
                var response = Task.FromResult(new OrganDonationResponse<ReferenceDataResponse>(httpStatus));
                var mockCacheEntry = new Mock<ICacheEntry>();
                mockCacheEntry.SetupProperty(x => x.AbsoluteExpiration);
                mockCacheEntry.SetupProperty(x => x.Value);
                
                mockCacheEntry.Object.Value = cached;

                if (throwException)
                {
                    mockClient.Setup(x => x.GetAllReferenceData()).Throws<HttpRequestException>();
                }
                else
                {
                    mockClient.Setup(x => x.GetAllReferenceData()).Returns(response);
                }
                
                mockCache.Setup(x => x.TryGetValue(ReferenceDataCacheKey, out It.Ref<object>.IsAny))
                    .Callback(new TryGetValueCallback((object key, out object value) =>
                    {
                        value = mockCacheEntry.Object.Value;
                    }))
                    .Returns(() => mockCacheEntry.Object.Value != null);
                
                mockCache.Setup(x => x.CreateEntry(ReferenceDataCacheKey))
                    .Returns(mockCacheEntry.Object);
                mockConfig.Setup(x => x.ReferenceDataExpiryHours)
                    .Returns(ReferenceDataExpiryHours);

                CacheEntry = mockCacheEntry.Object;
            }

            private ICacheEntry CacheEntry { get; }
            public DateTimeOffset? AbsoluteExpiration => CacheEntry.AbsoluteExpiration;
            public object Cached  => CacheEntry.Value;
        }
    }
}