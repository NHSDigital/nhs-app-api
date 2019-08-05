using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    public class TppLinkageService : ILinkageService
    {
        private readonly ITppClient _tppClient;
        private readonly ITppLinkageMapper _linkageMapper;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;
        private readonly IIm1CacheService _im1CacheService;
        private readonly ILogger<TppLinkageService> _logger;
        private readonly IMinimumAgeValidator _minimumAgeValidator;
        private readonly ConfigurationSettings _settings;

        public TppLinkageService(ITppClient client,
            ITppLinkageMapper linkageMapper,
            IIm1CacheKeyGenerator im1CacheKeyGenerator,
            IIm1CacheService im1CacheService,
            ILogger<TppLinkageService> logger,
            IMinimumAgeValidator minimumAgeValidator,
            ConfigurationSettings settings)
        {
            _tppClient = client;
            _linkageMapper = linkageMapper;
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
            _im1CacheService = im1CacheService;
            _logger = logger;
            _minimumAgeValidator = minimumAgeValidator;
            _settings = settings;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            return await Task.FromResult(new LinkageResult.NotFound(Im1ConnectionErrorCodes.InternalCode.UnknownError));
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            try
            {
                _logger.LogEnter();
                var request = CreateRequest(createLinkageRequest);

                var createNhsUserResponse = await _tppClient.NhsUserPost(request);

                if (createNhsUserResponse.HasSuccessResponse)
                {
                    _logger.LogInformation("Linkage Key Successfully created");
                    return await HandleCreateSuccess(request, createNhsUserResponse);
                }

                return HandleCreateError(createNhsUserResponse, request);

            }
            catch (Exception e) when (e is HttpRequestException || e is UnauthorisedGpSystemHttpRequestException)
            {
                _logger.LogError(e, "Unsuccessful request creating linkage key");
                return new LinkageResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private LinkageResult HandleCreateError(
            TppClient.TppApiObjectResponse<AddNhsUserResponse> createNhsUserResponse, AddNhsUserRequest request)
        {
            if (createNhsUserResponse.HasErrorWithCode("8") &&
                !_minimumAgeValidator.IsValid(request.DateofBirth, _settings.MinimumLinkageAge))
            {
                return new LinkageResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode.UnderMinimumAgeOrNonCompetent);
            }
            return TppLinkagePostErrorMapper.Map(createNhsUserResponse, _logger);
        }

        private async Task<LinkageResult> HandleCreateSuccess(AddNhsUserRequest request,
            TppClient.TppApiObjectResponse<AddNhsUserResponse> createNhsUserResponse)
        {
            try
            {
                var linkage = _linkageMapper.Map(request, createNhsUserResponse.Body);
                var connectionToken = CreateConnectionToken(createNhsUserResponse);
                await StoreAccessGuidInCache(linkage, connectionToken);
                return new LinkageResult.SuccessfullyCreated(linkage);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"Something went wrong during building the response. Exception message: {e.Message}");
                return new LinkageResult.InternalServerError();
            }

        }

        private static AddNhsUserRequest CreateRequest(CreateLinkageRequest createLinkageRequest)
        {
            var request = new AddNhsUserRequest
            {
                NhsNumber = createLinkageRequest.NhsNumber,
                DateofBirth = createLinkageRequest.DateOfBirth.Value,
                LastName = createLinkageRequest.Surname,
                OrganisationCode = createLinkageRequest.OdsCode
            };
            return request;
        }

        private static TppConnectionToken CreateConnectionToken(TppClient.TppApiObjectResponse<AddNhsUserResponse> createNhsUserResponse)
        {
            return new TppConnectionToken
            {
                AccountId = createNhsUserResponse.Body.AccountId,
                Passphrase = createNhsUserResponse.Body.Passphrase,
                ProviderId = createNhsUserResponse.Body.ProviderId
            };
        }

        private async Task StoreAccessGuidInCache(LinkageResponse linkage, TppConnectionToken im1ConnectionToken)
        {
            var key = _im1CacheKeyGenerator.GenerateCacheKey(
                linkage.AccountId,
                linkage.OdsCode,
                linkage.LinkageKey);

            im1ConnectionToken.Im1CacheKey = key;

            await _im1CacheService.SaveIm1ConnectionToken(key, im1ConnectionToken);
        }
    }
}
