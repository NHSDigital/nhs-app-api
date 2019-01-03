using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Im1Connection
{
    public class TppIm1ConnectionService : IIm1ConnectionService
    {
        private readonly ITppClient _tppClient;
        private readonly IIm1CacheService _im1CacheService;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;
        private readonly ILogger<TppIm1ConnectionService> _logger;

        public TppIm1ConnectionService(ITppClient tppClient, IIm1CacheService im1CacheService,
            IIm1CacheKeyGenerator im1CacheKeyGenerator, ILogger<TppIm1ConnectionService> logger)
        {
            _tppClient = tppClient;
            _im1CacheService = im1CacheService;
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
            _logger = logger;
        }

        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            try
            {
                _logger.LogEnter();

                var authenticateRequest = connectionToken.DeserializeJson<Authenticate>();
                authenticateRequest.UnitId = odsCode;

                var authenticateReply = await _tppClient.AuthenticatePost(authenticateRequest);

                if (!authenticateReply.HasSuccessResponse)
                {
                    _logger.LogError($"Tpp Authentication call failed - {authenticateReply.ErrorForLogging}");
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var nhsNumbers = authenticateReply.Body?.ExtractNhsNumbers() ?? Enumerable.Empty<PatientNhsNumber>();

                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken,
                    NhsNumbers = nhsNumbers
                };

                _logger.LogDebug($"{nameof(TppIm1ConnectionService)} Verify successfully completed");
                return new Im1ConnectionVerifyResult.SuccessfullyVerified(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e,
                    "Failed request to verify Tpp Im1ConnectionToken,HttpRequestException has been thrown.");
                return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            try
            {
                _logger.LogEnter();

                var linkAccountRequest = new LinkAccount
                {
                    AccountId = request.AccountId,
                    DateofBirth = request.DateOfBirth,
                    LastName = request.Surname,
                    OrganisationCode = request.OdsCode,
                    Passphrase = request.LinkageKey
                };

                _logger.LogDebug("Checking cache for IM1 connection token");
                var key = _im1CacheKeyGenerator.GenerateCacheKey(
                    request.AccountId, request.OdsCode, request.LinkageKey);
                var connectionTokenOption = await _im1CacheService.GetIm1ConnectionToken<TppConnectionToken>(key);

                TppConnectionToken connectionToken;

                if (connectionTokenOption.HasValue)
                {
                    connectionToken = connectionTokenOption.ValueOrFailure();
                    _logger.LogDebug("IM1 connection token found in cache.");
                }
                else
                {
                    var linkAccountReply = await _tppClient.LinkAccountPost(linkAccountRequest);

                    var standardErrorMessage = "Failed LinkAccount request returned with";
                    if (linkAccountReply.HasErrorWithCode(TppApiErrorCodes.LinkAccount.InvalidProviderId))
                    {
                        _logger.LogError($"{standardErrorMessage} '{nameof(TppApiErrorCodes.LinkAccount.InvalidProviderId)}' error code.");
                        return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                    }

                    if (linkAccountReply.HasErrorWithCode(TppApiErrorCodes.LinkAccount.InvalidLinkageCredentials))
                    {
                        _logger.LogError(
                            $"{standardErrorMessage} '{nameof(TppApiErrorCodes.LinkAccount.InvalidLinkageCredentials)}' error code.");
                        return new Im1ConnectionRegisterResult.NotFound();
                    }

                    if (!linkAccountReply.HasSuccessResponse)
                    {
                        _logger.LogError(
                            $"{standardErrorMessage}out success response.  Unknown Reason.");
                        return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                    }

                    connectionToken = new TppConnectionToken
                    {
                        AccountId = linkAccountRequest.AccountId,
                        Passphrase = linkAccountReply.Body.Passphrase,
                        ProviderId = linkAccountReply.Body.ProviderId,
                        Im1CacheKey = key
                    };

                    await CacheConnectionToken(connectionToken);
                }

                var authenticateRequest = new Authenticate
                {
                    AccountId = connectionToken.AccountId,
                    Passphrase = connectionToken.Passphrase,
                    UnitId = linkAccountRequest.OrganisationCode,
                    ProviderId = connectionToken.ProviderId
                };

                var authenticateReply = await _tppClient.AuthenticatePost(authenticateRequest);

                if (!authenticateReply.HasSuccessResponse)
                {
                    _logger.LogError(
                        "Failed Authenticate request returned without success response.  Unknown Reason.");
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var nhsNumbers = authenticateReply.Body?.ExtractNhsNumbers() ?? Enumerable.Empty<PatientNhsNumber>();

                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken.SerializeJson(),
                    NhsNumbers = nhsNumbers
                };

                _logger.LogDebug($"{nameof(TppIm1ConnectionService)} {nameof(Register)} successfully completed");
                return new Im1ConnectionRegisterResult.SuccessfullyRegistered(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e,
                    $"Failed request to TppIm1ConnectionToken.{nameof(Register)}, HttpRequestException has been thrown.");
                return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task CacheConnectionToken(TppConnectionToken im1ConnectionToken) =>
            await _im1CacheService.SaveIm1ConnectionToken(im1ConnectionToken.Im1CacheKey,
                im1ConnectionToken);
    }
}