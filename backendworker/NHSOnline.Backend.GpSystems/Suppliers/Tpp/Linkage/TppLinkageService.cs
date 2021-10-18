using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Cache;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Linkage;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    internal class TppLinkageService : ILinkageService
    {
        private readonly ITppClientRequest<LinkAccountRetrieve, LinkAccountReply> _linkAccountRetrieveRequest;
        private readonly ITppClientRequest<LinkAccountCreate, LinkAccountReply> _linkAccountCreateRequest;
        private readonly ITppLinkageMapper _linkageMapper;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;
        private readonly IIm1CacheService _im1CacheService;
        private readonly ILogger<TppLinkageService> _logger;
        private readonly IMinimumAgeValidator _minimumAgeValidator;
        private readonly ConfigurationSettings _settings;

        public TppLinkageService(
            ITppClientRequest<LinkAccountRetrieve, LinkAccountReply> linkAccountRetrieveRequest,
            ITppClientRequest<LinkAccountCreate, LinkAccountReply> linkAccountCreateRequest,
            ITppLinkageMapper linkageMapper,
            IIm1CacheKeyGenerator im1CacheKeyGenerator,
            IIm1CacheService im1CacheService,
            ILogger<TppLinkageService> logger,
            IMinimumAgeValidator minimumAgeValidator,
            ConfigurationSettings settings)
        {
            _linkAccountRetrieveRequest = linkAccountRetrieveRequest;
            _linkAccountCreateRequest = linkAccountCreateRequest;
            _linkageMapper = linkageMapper;
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
            _im1CacheService = im1CacheService;
            _logger = logger;
            _minimumAgeValidator = minimumAgeValidator;
            _settings = settings;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            try
            {
                var linkAccountRequest = CreateGetRequest(getLinkageRequest);
                var linkAccountReply = await _linkAccountRetrieveRequest.Post(linkAccountRequest);

                if (linkAccountReply.HasSuccessResponse)
                {
                    _logger.LogInformation("Linkage Key call successful");
                    return HandleRetrieveSuccess(linkAccountRequest, linkAccountReply);
                }

                return HandleRetrieveError(linkAccountReply, linkAccountRequest);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request get linkage key");
                return new LinkageResult.SupplierSystemUnavailable();
            }
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            try
            {
                _logger.LogEnter();
                var request = CreateRequest(createLinkageRequest);

                var createNhsUserResponse = await _linkAccountCreateRequest.Post(request);

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
            TppApiObjectResponse<LinkAccountReply> createNhsUserResponse, LinkAccountCreate request)
        {
            if (createNhsUserResponse.HasErrorWithCode("8") &&
                !_minimumAgeValidator.IsValid(request.DateofBirth, _settings.MinimumLinkageAge))
            {
                return new LinkageResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode.UnderMinimumAgeOrNonCompetent);
            }
            return TppLinkagePostErrorMapper.Map(createNhsUserResponse, _logger);
        }

        private async Task<LinkageResult> HandleCreateSuccess(LinkAccountCreate request,
            TppApiObjectResponse<LinkAccountReply> createNhsUserResponse)
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
                _logger.LogError(e, $"Something went wrong during building the response. Exception message: {e.Message}");
                return new LinkageResult.InternalServerError();
            }
        }

        private LinkageResult HandleRetrieveSuccess(LinkAccountRetrieve linkAccountRequest,
            TppApiObjectResponse<LinkAccountReply> linkAccountResponse)
        {
            try
            {
                bool validAccountDetailsFound = new ValidateAndLog(_logger)
                    .IsNotNullOrWhitespace(linkAccountResponse.Body.AccountId, nameof(linkAccountResponse.Body.AccountId))
                    .IsNotNullOrWhitespace(linkAccountResponse.Body.PassphraseToLink, nameof(linkAccountResponse.Body.PassphraseToLink))
                    .IsValid();

                if (!validAccountDetailsFound)
                {
                    _logger.LogInformation("Patient found but does not have an online account");
                    return new LinkageResult.NotFound(Im1ConnectionErrorCodes.InternalCode.NotValidForOnlineUser);
                }

                var linkage = _linkageMapper.Map(linkAccountRequest, linkAccountResponse.Body);
                return new LinkageResult.SuccessfullyRetrieved(linkage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong during building the response. Exception message: {e.Message}");
                return new LinkageResult.InternalServerError();
            }
        }

        private LinkageResult HandleRetrieveError(
            TppApiObjectResponse<LinkAccountReply> linkAccountReply, LinkAccount request)
        {
            if (linkAccountReply.HasErrorWithCode("8") &&
                !_minimumAgeValidator.IsValid(request.DateofBirth, _settings.MinimumLinkageAge))
            {
                return new LinkageResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode.UnderMinimumAgeOrNonCompetent);
            }
            return TppLinkageGetErrorMapper.Map(linkAccountReply, _logger);
        }

        private static LinkAccountCreate CreateRequest(CreateLinkageRequest createLinkageRequest)
        {
            var request = new LinkAccountCreate
            {
                NhsNumber = createLinkageRequest.NhsNumber,
                DateofBirth = createLinkageRequest.DateOfBirth.Value,
                LastName = createLinkageRequest.Surname,
                OrganisationCode = createLinkageRequest.OdsCode,
                EmailAddress = createLinkageRequest.EmailAddress,
                MobileNumber = createLinkageRequest.PhoneNumber,
            };
            return request;
        }

        private static LinkAccountRetrieve CreateGetRequest(GetLinkageRequest getLinkageRequest)
        {
            return new LinkAccountRetrieve
            {
                NhsNumber = getLinkageRequest.NhsNumber,
                DateofBirth = getLinkageRequest.DateOfBirth.Value,
                LastName = getLinkageRequest.Surname,
                OrganisationCode = getLinkageRequest.OdsCode,
            };
        }

        private static TppConnectionToken CreateConnectionToken(TppApiObjectResponse<LinkAccountReply> createNhsUserResponse)
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
