using System.Net;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Linkage
{
    public class MicrotestLinkageService : ILinkageService
    {
        private readonly IMicrotestClient _microtestClient;
        private readonly ILogger<MicrotestLinkageService> _logger;

        public const string TemporaryAccountId = "MICROTEST_ACCOUNT_ID";
        public const string TemporaryLinkageKey = "MICROTEST_LINKAGE_KEY";

        public MicrotestLinkageService(IMicrotestClient microtestClient, ILogger<MicrotestLinkageService> logger)
        {
            _microtestClient = microtestClient;
            _logger = logger;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            var demographicsResponse = await _microtestClient.DemographicsGet(
                    getLinkageRequest.OdsCode,
                    getLinkageRequest.NhsNumber);
            
            if (!demographicsResponse.HasSuccessResponse)
            {
                _logger.LogError($"Unsuccessful request retrieving demographics as part of linkage. Status code: {(int)demographicsResponse.StatusCode}");
                return new LinkageResult.UnmappedErrorWithStatusCode(HttpStatusCode.BadGateway);
            }

            var linkage = await Task.FromResult(new LinkageResponse
            {
                AccountId = TemporaryAccountId,
                LinkageKey = TemporaryLinkageKey,
                OdsCode = getLinkageRequest.OdsCode
            });

            return new LinkageResult.SuccessfullyRetrieved(linkage);
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            return await Task.FromResult(new LinkageResult.NotFound(Im1ConnectionErrorCodes.InternalCode.UnknownError));
        }
    }
}