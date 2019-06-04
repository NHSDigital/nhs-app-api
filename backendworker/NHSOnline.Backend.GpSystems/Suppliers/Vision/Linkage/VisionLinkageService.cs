using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage
{
    public class VisionLinkageService : ILinkageService
    {
        private readonly ILogger<VisionLinkageService> _logger;
        private readonly IVisionClient _visionClient;
        private readonly IVisionLinkageMapper _visionLinkageMapper;
        private readonly VisionLinkageGetErrorMapper _getErrorMapper;
        private readonly VisionLinkagePostErrorMapper _postErrorMapper;

        public VisionLinkageService(
            ILoggerFactory loggerFactory,
            IVisionClient visionClient,
            IVisionLinkageMapper visionLinkageMapper,
            VisionLinkageGetErrorMapper getErrorMapper,
            VisionLinkagePostErrorMapper postErrorMapper)
        {
            _logger = loggerFactory.CreateLogger<VisionLinkageService>();
            _visionClient = visionClient;
            _visionLinkageMapper = visionLinkageMapper;
            _getErrorMapper = getErrorMapper;
            _postErrorMapper = postErrorMapper;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            try
            {
                _logger.LogEnter();

                var request = BuildGetLinkageKey(getLinkageRequest);

                var response = await _visionClient.GetLinkageKey(request);

                return response.HasSuccessResponse ?
                    HandleSuccessfulGetLinkageKey(response) :
                    _getErrorMapper.Map(response, _logger);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving linkage key");
                return new LinkageResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            try
            {
                _logger.LogEnter();

                var request = BuildCreateLinkageKey(createLinkageRequest);

                var linkageResponse = await _visionClient.CreateLinkageKey(request);
                
                if (linkageResponse.HasSuccessResponse)
                {
                    var mapped = _visionLinkageMapper.Map(linkageResponse.Body);
                    return new LinkageResult.SuccessfullyCreated(mapped);
                }

                return _postErrorMapper.Map(linkageResponse, _logger);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request creating linkage key");
                return new LinkageResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private LinkageResult HandleSuccessfulGetLinkageKey(
            VisionLinkageClient.VisionApiObjectResponse<LinkageKeyGetResponse> response)
        {
            try
            {
                var mapped = _visionLinkageMapper.Map(response.Body);

                return new LinkageResult.SuccessfullyRetrieved(mapped);
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong during building the response. Exception message: {e.Message}");
                return new LinkageResult.InternalServerError();
            }
        }

        private static GetLinkageKey BuildGetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            return new GetLinkageKey
            {
                NhsNumber = getLinkageRequest.NhsNumber,
                OdsCode = getLinkageRequest.OdsCode,
            };
        }

        private static CreateLinkageKey BuildCreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            return new CreateLinkageKey
            {
                OdsCode = createLinkageRequest.OdsCode,
                LinkageKeyPostRequest = new LinkageKeyPostRequest
                {
                    NhsNumber = createLinkageRequest.NhsNumber,
                    LastName = createLinkageRequest.Surname,
                    DateOfBirth = createLinkageRequest.DateOfBirth.FormatToYYYYMMDD(),
                },
            };
        }
    }
}