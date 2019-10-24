using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection
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
                    return TppIm1VerifyErrorMapper.Map(authenticateReply, _logger);
                }
                
                var response = CreatePatientIm1ConnectionResponse(authenticateReply, connectionToken, odsCode);

                _logger.LogDebug($"{nameof(TppIm1ConnectionService)} Verify successfully completed");
                return new Im1ConnectionVerifyResult.Success(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e,
                    "Failed request to verify Tpp Im1ConnectionToken,HttpRequestException has been thrown.");
                return new Im1ConnectionVerifyResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private static PatientIm1ConnectionResponse CreatePatientIm1ConnectionResponse(
            TppClient.TppApiObjectResponse<AuthenticateReply> authenticateReply, string connectionToken, string odsCode)
        {
            var nhsNumbers = authenticateReply.Body?.ExtractNhsNumbers() ?? Enumerable.Empty<PatientNhsNumber>();

            return new PatientIm1ConnectionResponse
            {
                ConnectionToken = connectionToken,
                NhsNumbers = nhsNumbers,
                OdsCode = odsCode
            };
        }

        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            try
            {
                _logger.LogEnter();

                var linkAccountRequest = CreateLinkAccount(request);

                _logger.LogInformation("Checking cache for IM1 connection token");
                var key = _im1CacheKeyGenerator.GenerateCacheKey(
                    request.AccountId, request.OdsCode, request.LinkageKey);
                var connectionTokenOption = await _im1CacheService.GetIm1ConnectionToken<TppConnectionToken>(key);

                TppConnectionToken connectionToken;

                if (connectionTokenOption.HasValue)
                {
                    connectionToken = connectionTokenOption.ValueOrFailure();
                    _logger.LogInformation("IM1 connection token found in cache.");
                }
                else
                {
                    _logger.LogInformation("IM1 connection token not found in cache.");
                    var linkAccountReply = await _tppClient.LinkAccountPost(linkAccountRequest);

                    if (!linkAccountReply.HasSuccessResponse)
                    {
                        return TppIm1RegisterErrorMapper.Map(linkAccountReply, _logger);
                    }

                    connectionToken = CreateTppConnectionToken(linkAccountRequest, linkAccountReply, key);

                    await CacheConnectionToken(connectionToken);
                }
                
                return await HandleAuthenticateRequestAndResponse(connectionToken, linkAccountRequest);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e,
                    $"Failed request to TppIm1ConnectionToken.{nameof(Register)}, HttpRequestException has been thrown.");
                return new Im1ConnectionRegisterResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<Im1ConnectionRegisterResult> HandleAuthenticateRequestAndResponse(TppConnectionToken connectionToken,
            LinkAccount linkAccountRequest)
        {

            var authenticateRequest = CreateAuthenticate(connectionToken, linkAccountRequest);

            var authenticateReply = await _tppClient.AuthenticatePost(authenticateRequest);

            if (!authenticateReply.HasSuccessResponse)
            {
                _logger.LogError(
                    "Failed Authenticate request returned without success response.  Unknown Reason.");
                return new Im1ConnectionRegisterResult.BadGateway();
            }

            var nhsNumbers = authenticateReply.Body?.ExtractNhsNumbers() ?? Enumerable.Empty<PatientNhsNumber>();

            var response = new CreateIm1ConnectionResponse
            {
                ConnectionToken = connectionToken.SerializeJson(),
                NhsNumbers = nhsNumbers,
                OdsCode = linkAccountRequest.OrganisationCode,
                AccountId = linkAccountRequest.AccountId,
                LinkageKey = linkAccountRequest.Passphrase
            };

            _logger.LogDebug($"{nameof(TppIm1ConnectionService)} {nameof(Register)} successfully completed");
            return new Im1ConnectionRegisterResult.Success(response);

        }

        private static TppConnectionToken CreateTppConnectionToken(LinkAccount linkAccountRequest,
            TppClient.TppApiObjectResponse<LinkAccountReply> linkAccountReply, string key) {
            return new TppConnectionToken
            {
                AccountId = linkAccountRequest.AccountId,
                Passphrase = linkAccountReply.Body.Passphrase,
                ProviderId = linkAccountReply.Body.ProviderId,
                Im1CacheKey = key
            };
        }

        private static Authenticate CreateAuthenticate(TppConnectionToken connectionToken, LinkAccount linkAccountRequest)
        {
            return new Authenticate
            {
                AccountId = connectionToken.AccountId,
                Passphrase = connectionToken.Passphrase,
                UnitId = linkAccountRequest.OrganisationCode,
                ProviderId = connectionToken.ProviderId
            };

        }

        private static LinkAccount CreateLinkAccount(PatientIm1ConnectionRequest request)
        {
            return new LinkAccount
            {
                AccountId = request.AccountId,
                DateofBirth = request.DateOfBirth,
                LastName = request.Surname,
                OrganisationCode = request.OdsCode,
                Passphrase = request.LinkageKey
            };
        }

        private async Task CacheConnectionToken(TppConnectionToken im1ConnectionToken) =>
            await _im1CacheService.SaveIm1ConnectionToken(im1ConnectionToken.Im1CacheKey,
                im1ConnectionToken);
    }
}
