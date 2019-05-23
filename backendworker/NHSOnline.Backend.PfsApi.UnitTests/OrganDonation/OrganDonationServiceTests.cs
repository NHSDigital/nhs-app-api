using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationServiceTests
    {
        private const string ReferenceDataCacheKey = "_organDonationReferenceData";
        private const int ReferenceDataExpirySeconds = 21600;
        private OrganDonationService _organDonationService;
        private UserSession _userSession;
        private Mock<IMemoryCache> _mockMemoryCache;
        private Mock<IOrganDonationClient> _mockOrganDonationClient;
        private Mock<IOrganDonationConfig> _mockOrganDonationConfig;
        private Mock<IMapper<OrganDonationRegistration, RegistrationLookupRequest>>
            _mockLookupRegistrationRequestMapper;

        private Mock<IMapper<OrganDonationRegistrationRequest, RegistrationRequest>>
            _mockRegistrationRequestMapper;

        private Mock<IMapper<OrganDonationWithdrawRequest, WithdrawRequest>>
            _mockRegistrationWithdrawMapper;

        private Mock<IMapper<HttpStatusCode, OrganDonationResult>> 
            _mockResultErrorMapper;
        
        private Mock<IMapper<HttpStatusCode, OrganDonationRegistrationResult>> 
            _mockRegistrationResultErrorMapper;

        private Mock<IMapper<HttpStatusCode, OrganDonationReferenceDataResult>>
            _mockReferenceDataResultErrorMapper;

        private Mock<IMapper<HttpStatusCode, OrganDonationWithdrawResult>>
            _mockWithdrawResultErrorMapper;
        

        // needed for Callback
        private delegate void TryGetValueCallback(object key, out object value);

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockMemoryCache = fixture.Freeze<Mock<IMemoryCache>>();
            _mockOrganDonationClient = fixture.Freeze<Mock<IOrganDonationClient>>();
            _mockOrganDonationConfig = fixture.Freeze<Mock<IOrganDonationConfig>>();
            _mockLookupRegistrationRequestMapper =
                fixture.Freeze<Mock<IMapper<OrganDonationRegistration, RegistrationLookupRequest>>>();

            _mockRegistrationRequestMapper =
                fixture.Freeze<Mock<IMapper<OrganDonationRegistrationRequest, RegistrationRequest>>>();

            _mockRegistrationWithdrawMapper =
                fixture.Freeze<Mock<IMapper<OrganDonationWithdrawRequest, WithdrawRequest>>>();

            _userSession = fixture.Create<UserSession>();
            _mockResultErrorMapper =
                fixture.Freeze<Mock<IMapper<HttpStatusCode, OrganDonationResult>>>();
            _mockRegistrationResultErrorMapper =
                fixture.Freeze<Mock<IMapper<HttpStatusCode, OrganDonationRegistrationResult>>>();
            _mockReferenceDataResultErrorMapper =
                fixture.Freeze<Mock<IMapper<HttpStatusCode, OrganDonationReferenceDataResult>>>();
            _mockWithdrawResultErrorMapper =
                fixture.Freeze<Mock<IMapper<HttpStatusCode, OrganDonationWithdrawResult>>>();
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
        public void GetOrganDonation_WhenCalledAndSearchFailsWithException_ReturnsSearchErrorResponse()
        {
            // Arrange 
            var context = SetupGetOrganDonationTest(true);

            // Act
            var result = _organDonationService.GetOrganDonation(context.DemographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationResult.SearchError>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.MethodNotAllowed)]
        [DataRow(HttpStatusCode.UnsupportedMediaType)]
        [DataRow(HttpStatusCode.UnprocessableEntity)]
        [DataRow(HttpStatusCode.TooManyRequests)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.RequestTimeout)]
        public void GetOrganDonation_WhenCalledAndSearchFailsWithError_MapsSearchErrorResponse(
            HttpStatusCode httpStatus)
        {
            // Arrange 
            var context = SetupGetOrganDonationTest(httpStatus: httpStatus);

            // Act
            var _ = _organDonationService.GetOrganDonation(context.DemographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), _userSession));
            _mockResultErrorMapper.Verify(x => x.Map(httpStatus));
        }

        [TestMethod]
        public void
            GetOrganDonation_WhenCalledAndDemographicsRetrievedUnsuccessfully_ReturnsDemographicsBadGateway()
        {
            // Arrange 
            var demographicsResult = new DemographicsResult.BadGateway();

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.VerifyNoOtherCalls();
            result.Result.Should().BeOfType<OrganDonationResult.DemographicsBadGateway>();
        }

        [TestMethod]
        public void
            GetOrganDonation_WhenCalledAndDemographicsForbidden_ReturnsDemographicsForbidden()
        {
            // Arrange 
            var demographicsResult = new DemographicsResult.Forbidden();

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
            GetOrganDonation_WhenCalledAndDemographicsBadGateway_ReturnsDemographicsBadGateway()
        {
            // Arrange 
            var demographicsResult = new DemographicsResult.BadGateway();

            // Act
            var result = _organDonationService.GetOrganDonation(demographicsResult, _userSession);

            // Assert
            _mockOrganDonationClient.VerifyNoOtherCalls();
            result.Result.Should().BeOfType<OrganDonationResult.DemographicsBadGateway>();
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
            context.AbsoluteExpiration.Should().BeCloseTo(DateTimeOffset.UtcNow.AddSeconds(ReferenceDataExpirySeconds),
                TimeSpan.FromSeconds(1));
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
        [DataRow(HttpStatusCode.RequestTimeout)]
        public void
            GetReferenceData_WhenCalledAndRetrievalFailsWithError_MapsErrorResponse(HttpStatusCode httpStatus)
        {
            // Arrange
            var context = SetupGetReferenceDataTest(httpStatus: httpStatus);

            // Act
            var result = _organDonationService.GetReferenceData();

            // Assert
            _mockOrganDonationClient.Verify(x => x.GetAllReferenceData());
            _mockReferenceDataResultErrorMapper.Verify(x => x.Map(httpStatus));
            
            context.AbsoluteExpiration.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
            result.Result.Should().BeEquivalentTo(context.Cached);
        }

        [TestMethod]
        public void
            GetReferenceData_WhenCalledAndRetrievalFailsWithException_ReturnSystemErrorResponseWithNoCaching()
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
            var result = _organDonationService.Register(context.Request, _userSession);

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
        [DataRow(HttpStatusCode.RequestTimeout)]
        public void Register_WhenCalledAndRetrievalFailsWithError_MapsErrorResponse(HttpStatusCode httpStatus)
        {
            // Arrange
            var context = SetupRegistrationTest(httpStatus: httpStatus);

            // Act
            var _ = _organDonationService.Register(context.Request, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostRegistration(It.IsAny<RegistrationRequest>(), _userSession));                                  
            _mockRegistrationResultErrorMapper.Verify(x => x.Map(httpStatus));
        }

        [TestMethod]
        public void Register_WhenCalledAndRetrievalFailsWithException_ReturnSystemErrorResponseWithNoCaching()
        {
            // Arrange
            var context = SetupRegistrationTest(true);

            // Act
            var result = _organDonationService.Register(context.Request, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PostRegistration(It.IsAny<RegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationRegistrationResult.SystemError>();
        }

        [TestMethod]
        public void Update_WhenCalledWithRequest_ReturnsSuccessfullyUpdatedResponse()
        {
            // Arrange 
            var context = SetupUpdateTest();

            // Act
            var result = _organDonationService.Update(context.Request, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PutUpdate(It.IsAny<RegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationRegistrationResult.SuccessfullyRegistered>();
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
        [DataRow(HttpStatusCode.RequestTimeout)]
        public void Update_WhenCalledAndRetrievalFailsWithError_MapsErrorResponse(HttpStatusCode httpStatus)
        {
            // Arrange
            var context = SetupUpdateTest(httpStatus: httpStatus);

            // Act
            var _ = _organDonationService.Update(context.Request, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PutUpdate(It.IsAny<RegistrationRequest>(), _userSession));
            _mockRegistrationResultErrorMapper.Verify(x => x.Map(httpStatus));
        }

        [TestMethod]
        public void Update_WhenCalledAndRetrievalFailsWithException_ReturnSystemErrorResponse()
        {
            // Arrange
            var context = SetupUpdateTest(true);

            // Act
            var result = _organDonationService.Update(context.Request, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.PutUpdate(It.IsAny<RegistrationRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationRegistrationResult.SystemError>();
        }

        [TestMethod]
        public void Withdraw_WhenCalledWithRequest_ReturnsSuccessfullyDeletedResponse()
        {
            // Arrange 
            var context = SetupWithdrawTest();

            // Act
            var result = _organDonationService.Withdraw(context.Request, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.Delete(It.IsAny<WithdrawRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationWithdrawResult.SuccessfullyWithdrawn>();
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
        [DataRow(HttpStatusCode.RequestTimeout)]
        public void Withdraw_WhenCalledAndRetrievalFailsWithError_MapsErrorResponse(HttpStatusCode httpStatus)
        {
            // Arrange
            var context = SetupWithdrawTest(httpStatus: httpStatus);

            // Act
            var _ = _organDonationService.Withdraw(context.Request, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.Delete(It.IsAny<WithdrawRequest>(), _userSession));
            _mockWithdrawResultErrorMapper.Verify(x => x.Map(httpStatus));
        }

        [TestMethod]
        public void Withdraw_WhenCalledAndRetrievalFailsWithException_ReturnSystemErrorResponse()
        {
            // Arrange
            var context = SetupWithdrawTest(true);

            // Act
            var result = _organDonationService.Withdraw(context.Request, _userSession);

            // Assert
            _mockOrganDonationClient.Verify(x => x.Delete(It.IsAny<WithdrawRequest>(), _userSession));

            result.Result.Should().BeOfType<OrganDonationWithdrawResult.SystemError>();
        }

        private GetReferenceDataTestContext SetupGetReferenceDataTest(
            bool throwException = false,
            HttpStatusCode httpStatus = HttpStatusCode.OK,
            OrganDonationReferenceDataResult cached = null
        )
        {
            return new GetReferenceDataTestContext(_mockOrganDonationConfig, _mockOrganDonationClient, _mockMemoryCache,
                throwException, httpStatus, cached);
        }

        private GetOrganDonationTestContext SetupGetOrganDonationTest(
            bool throwException = false,
            HttpStatusCode httpStatus = HttpStatusCode.OK
        )
        {
            return new GetOrganDonationTestContext(_mockOrganDonationClient, _userSession, throwException, httpStatus, 
                _mockLookupRegistrationRequestMapper);
        }

        private UpdateTestContext SetupUpdateTest(
            bool throwException = false,
            HttpStatusCode httpStatus = HttpStatusCode.OK
        )
        {
            return new UpdateTestContext(_mockOrganDonationClient, _userSession, throwException, httpStatus,
                _mockRegistrationRequestMapper );
        }

        private RegistrationTestContext SetupRegistrationTest(
            bool throwException = false,
            HttpStatusCode httpStatus = HttpStatusCode.OK
        )
        {
            return new RegistrationTestContext(_mockOrganDonationClient, _userSession, throwException, httpStatus,
                _mockRegistrationRequestMapper );
        }

        private WithdrawContext SetupWithdrawTest(
            bool throwException = false,
            HttpStatusCode httpStatus = HttpStatusCode.OK
        )
        {
            return new WithdrawContext(
                x => x.Delete(It.IsAny<WithdrawRequest>(), _userSession),
                _mockOrganDonationClient,
                throwException,
                httpStatus,
                _mockRegistrationWithdrawMapper);
        }

        private class GetOrganDonationTestContext
        {
            public GetOrganDonationTestContext(
                Mock<IOrganDonationClient> mockClient,
                UserSession userSession,
                bool throwException,
                HttpStatusCode httpStatus,
                Mock<IMapper<OrganDonationRegistration, RegistrationLookupRequest>> mockLookupRegistrationRequestMapper)
            {
                var demographicsResponse = new DemographicsResponse();
                var organDonationResponse = new OrganDonationResponse<RegistrationLookupResponse>(httpStatus);
                
                DemographicsResult = new DemographicsResult.Success(demographicsResponse);

                SetupMockClient(x => x.PostLookup(It.IsAny<RegistrationLookupRequest>(), userSession),
                    mockClient,
                    throwException,
                    organDonationResponse);

                mockLookupRegistrationRequestMapper
                    .Setup(x => x.Map(It.IsAny<OrganDonationRegistration>()))
                    .Returns(new RegistrationLookupRequest());
            }

            public DemographicsResult DemographicsResult { get; }
        }

        private class UpdateTestContext : UpdateRegisterContext
        {
            public UpdateTestContext(
                Mock<IOrganDonationClient> mockClient,
                UserSession userSession,
                bool throwException,
                HttpStatusCode httpStatus,
                Mock<IMapper<OrganDonationRegistrationRequest, RegistrationRequest>> mockRegistrationRequestMapper
            ) : base(x => x.PutUpdate(It.IsAny<RegistrationRequest>(), userSession),
                mockClient, throwException, httpStatus, mockRegistrationRequestMapper)
            {
            }
        }

        private class RegistrationTestContext : UpdateRegisterContext
        {
            public RegistrationTestContext(
                Mock<IOrganDonationClient> mockClient,
                UserSession userSession,
                bool throwException,
                HttpStatusCode httpStatus,
                Mock<IMapper<OrganDonationRegistrationRequest, RegistrationRequest>> mockRegistrationRequestMapper
            ) : base(x => x.PostRegistration(It.IsAny<RegistrationRequest>(), userSession),
                mockClient, throwException, httpStatus, mockRegistrationRequestMapper)
            {
            }
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
                var response = new OrganDonationResponse<ReferenceDataResponse>(httpStatus);
                var mockCacheEntry = new Mock<ICacheEntry>();
                mockCacheEntry.SetupProperty(x => x.AbsoluteExpiration);
                mockCacheEntry.SetupProperty(x => x.Value);

                mockCacheEntry.Object.Value = cached;

                SetupMockClient(x => x.GetAllReferenceData(), mockClient, throwException, response);

                mockCache.Setup(x => x.TryGetValue(ReferenceDataCacheKey, out It.Ref<object>.IsAny))
                    .Callback(new TryGetValueCallback((object key, out object value) =>
                    {
                        value = mockCacheEntry.Object.Value;
                    }))
                    .Returns(() => mockCacheEntry.Object.Value != null);

                mockCache.Setup(x => x.CreateEntry(ReferenceDataCacheKey))
                    .Returns(mockCacheEntry.Object);
                mockConfig.Setup(x => x.ReferenceDataExpirySeconds)
                    .Returns(ReferenceDataExpirySeconds);

                CacheEntry = mockCacheEntry.Object;
            }

            private ICacheEntry CacheEntry { get; }
            public DateTimeOffset? AbsoluteExpiration => CacheEntry.AbsoluteExpiration;
            public object Cached => CacheEntry.Value;
        }

        private abstract class UpdateRegisterContext
        {
            protected UpdateRegisterContext(
                Expression<Func<IOrganDonationClient, Task<OrganDonationResponse<OrganDonationBasicResponse>>>>
                    actionToMock,
                Mock<IOrganDonationClient> mockClient,
                bool throwException,
                HttpStatusCode httpStatus,
                Mock<IMapper<OrganDonationRegistrationRequest, RegistrationRequest>> mockRegistrationRequestMapper
            )
            {
                Request = new OrganDonationRegistrationRequest();
                var organDonationResponse = new OrganDonationResponse<OrganDonationBasicResponse>(httpStatus);

                SetupMockClient(actionToMock, mockClient, throwException, organDonationResponse);

                mockRegistrationRequestMapper
                    .Setup(x => x.Map(It.IsAny<OrganDonationRegistrationRequest>()))
                    .Returns(new RegistrationRequest());
            }
            public OrganDonationRegistrationRequest Request { get; }
        }

        internal class WithdrawContext
            {
                internal WithdrawContext(
                    Expression<Func<IOrganDonationClient, Task<OrganDonationResponse<OrganDonationBasicResponse>>>> actionToMock,
                    Mock<IOrganDonationClient> mockClient,
                    bool throwException,
                    HttpStatusCode httpStatus,
                    Mock<IMapper<OrganDonationWithdrawRequest, WithdrawRequest>> mockRegistrationRequestMapper
                )
                {
                    Request = new OrganDonationWithdrawRequest();
                    var organDonationResponse = new OrganDonationResponse<OrganDonationBasicResponse>(httpStatus);

                    SetupMockClient(actionToMock, mockClient, throwException, organDonationResponse);

                    mockRegistrationRequestMapper
                        .Setup(x => x.Map(It.IsAny<OrganDonationWithdrawRequest>()))
                        .Returns(new WithdrawRequest());
                }

                public OrganDonationWithdrawRequest Request { get; }
        }

        private static void SetupMockClient<TResponse>(
            Expression<Func<IOrganDonationClient, Task<TResponse>>> actionToMock,
            Mock<IOrganDonationClient> mockClient,
            bool throwException,
            TResponse response
        )
        {
            if (throwException)
            {
                mockClient.Setup(actionToMock).Throws<HttpRequestException>();
            }
            else
            {
                mockClient.Setup(actionToMock).Returns(Task.FromResult(response));
            }
        }
    }
}