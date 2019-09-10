using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Im1Connection;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Im1Connection
{
    public class MicrotestIm1ConnectionService : IIm1ConnectionService
    {
        private readonly IMicrotestClient _microtestClient;
        private readonly IIm1CacheKeyGenerator _im1CacheKeyGenerator;
        private readonly IIm1CacheService _im1CacheService;
        private readonly ILogger<MicrotestIm1ConnectionService> _logger;
        
        public MicrotestIm1ConnectionService(
            IMicrotestClient microtestClient, 
            IIm1CacheKeyGenerator im1CacheKeyGenerator,
            IIm1CacheService im1CacheService,
            ILogger<MicrotestIm1ConnectionService> logger)
        {
            _microtestClient = microtestClient;
            _im1CacheKeyGenerator = im1CacheKeyGenerator;
            _im1CacheService = im1CacheService;
            _logger = logger;
        }
        
        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            _logger.LogEnter();
            try
            {
                
                var token = connectionToken.DeserializeJson<MicrotestConnectionToken>();
    
                var nhsNumber = token.NhsNumber;
                
                var demographicsResponse = await _microtestClient.DemographicsGet(
                    odsCode,
                    nhsNumber);
                
                if (!demographicsResponse.HasSuccessResponse)
                {
                    // add in the other ones from demographic service.
                    if (demographicsResponse.HasForbiddenResponse)
                    {
                        _logger.LogError($"User does not have the necessary permissions within the GP system.");
                        return new Im1ConnectionVerifyResult.Forbidden();
                    }
                    if (demographicsResponse.HasInternalServerError)
                    {
                        _logger.LogError(
                            $"Internal server error when retrieving demographics as part of linkage. Status code: {(int) demographicsResponse.StatusCode}");
                        return new Im1ConnectionVerifyResult.InternalServerError();
                    }
                    _logger.LogError($"Unsuccessful request retrieving demographics as part of linkage. Status code: {(int)demographicsResponse.StatusCode}");
                    return new Im1ConnectionVerifyResult.Forbidden();
                }

                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken,
                    NhsNumbers = new List<PatientNhsNumber>
                    {
                        new PatientNhsNumber
                        {
                            NhsNumber = nhsNumber,
                        }
                    },
                    OdsCode = odsCode

                };
                
                return new Im1ConnectionVerifyResult.Success(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e,
                    "Failed request to verify Microtest Im1ConnectionToken, HttpRequestException has been thrown.");
                return new Im1ConnectionVerifyResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            _logger.LogEnter();
            try 
            {
                var key = _im1CacheKeyGenerator.GenerateCacheKey(
                    request.AccountId, request.OdsCode, request.LinkageKey);
                
                var cachedConnectionToken =
                    await _im1CacheService.GetIm1ConnectionToken<MicrotestConnectionToken>(key);
                
                if (cachedConnectionToken.HasValue)
                {
                    var connectionToken = cachedConnectionToken.ValueOrFailure();
                    _logger.LogDebug("IM1 connection token found in cache.");

                    var response = new PatientIm1ConnectionResponse
                    {
                        ConnectionToken = connectionToken.SerializeJson(),
                        NhsNumbers = new List<PatientNhsNumber>
                        {
                            new PatientNhsNumber
                            {
                                NhsNumber = connectionToken.NhsNumber
                            }
                        },
                        OdsCode = request.OdsCode,
                        AccountId = request.AccountId,
                        LinkageKey = request.LinkageKey,
                    };
    
                    _logger.LogExit();
                    return new Im1ConnectionRegisterResult.Success(response);
                }
                
                _logger.LogDebug("IM1 connection token not found in cache.");
                _logger.LogExit();
                return new Im1ConnectionRegisterResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode
                    .LinkageKeysNotSupportedBySupplier);
                
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e,
                    "Failed request to verify Microtest Im1ConnectionToken, HttpRequestException has been thrown.");
                return new Im1ConnectionRegisterResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
    
}
