using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.Areas.Session;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.TppClient;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Linkage
{
    public class TppLinkageService : ILinkageService
    {
        private readonly ITppClient _tppClient;
        private readonly ITppLinkageMapper _linkageMapper;
        private readonly IRegistrationGuidKeyGenerator _registrationGuidKeyGenerator;
        private readonly IRegistrationCacheService _registrationCacheService;
        private readonly ILogger<TppLinkageService> _logger;
        private readonly IMinimumAgeValidator _minimumAgeValidator;

        public TppLinkageService(ITppClient client,
            ITppLinkageMapper linkageMapper,
            IRegistrationGuidKeyGenerator registrationGuidKeyGenerator,
            IRegistrationCacheService registrationCacheService,
            ILogger<TppLinkageService> logger,
            IMinimumAgeValidator minimumAgeValidator)
        {
            _tppClient = client;
            _linkageMapper = linkageMapper;
            _registrationGuidKeyGenerator = registrationGuidKeyGenerator;
            _registrationCacheService = registrationCacheService;
            _logger = logger;
            _minimumAgeValidator = minimumAgeValidator;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            return await Task.FromResult(new LinkageResult.NotFoundErrorRetrievingNhsUser());
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            try
            {
                var request = CreateRequest(createLinkageRequest);
                var createNhsUserResponse = await _tppClient.NhsUserPost(request);

                if (!createNhsUserResponse.HasSuccessResponse)
                {
                    var errors = new TppLinkageErrors(_minimumAgeValidator, _logger);
                    return errors.GetErrorCreatingNhsUser(createNhsUserResponse, request);
                }

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
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request creating linkage key");
                return new LinkageResult.SupplierSystemUnavailable();
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

        private static TppConnectionToken CreateConnectionToken(TppApiObjectResponse<AddNhsUserResponse> createNhsUserResponse)
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
            var key = _registrationGuidKeyGenerator.GenerateRegistrationKey(
                linkage.AccountId,
                linkage.OdsCode,
                linkage.LinkageKey);
            await _registrationCacheService.CreateRegistrationToken(key, im1ConnectionToken);
        }
    }
}