using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisLinkageService : ILinkageService
    {
        private readonly ILogger _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisLinkageMapper _emisLinkageMapper;

        public EmisLinkageService(
            ILoggerFactory loggerFactory,
            IEmisClient emisClient,
            IEmisLinkageMapper emisLinkageMapper)
        {
            _emisClient = emisClient;
            _emisLinkageMapper = emisLinkageMapper;
            _logger = loggerFactory.CreateLogger<EmisLinkageService>();
        }

        public async Task<GetLinkageResult> GetLinkageKey(string nhsNumber, string odsCode)
        {
            try
            {
                var linkageResponse = await _emisClient.LinkageGet(nhsNumber, odsCode);

                if (linkageResponse.HasSuccessStatusCode)
                {
                    try
                    {
                        _logger.LogDebug($"Mapping response from {nameof(LinkageDetailsResponse)} to {nameof(LinkageResponse)}");
                        var linkage = _emisLinkageMapper.Map(linkageResponse.Body);
                        
                        return new GetLinkageResult.SuccessfullyRetrieved(linkage);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Something went wrong during building the response. Exception message: {e.Message}");

                        return new GetLinkageResult.InternalServerError();
                    }
                }

                if (linkageResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogError("NHS number was not found");
                    return new GetLinkageResult.NhsNumberNotFound();
                }
                
                if (linkageResponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    _logger.LogError("Linkage key has been revoked");
                    return new GetLinkageResult.LinkageKeyRevoked();
                }

                _logger.LogError(
                    "Emis system is currently unavailable");

                return new GetLinkageResult.SupplierSystemUnavailable();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving linkage key");
                return new GetLinkageResult.SupplierSystemUnavailable();
            }
        }

        public async Task<CreateLinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            try
            {
                var request = new LinkagePostRequest
                {
                    NhsNumber = createLinkageRequest.NhsNumber,
                    OdsCode = createLinkageRequest.OdsCode,
                };

                var response = await _emisClient.LinkagePost(request);

                if (response.HasSuccessStatusCode)
                {
                    var result = _emisLinkageMapper.Map(response.Body);

                    return new CreateLinkageResult.SuccessfullyRetrieved(result);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError("NHS number was not found");
                    return new CreateLinkageResult.NhsNumberNotFound();
                }

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    _logger.LogError("NHS number already has a linkage key");
                    return new CreateLinkageResult.LinkageKeyAlreadyExists();
                }

                return new CreateLinkageResult.SupplierSystemUnavailable();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request creating linkage key");
                return new CreateLinkageResult.SupplierSystemUnavailable();
            }
        }
    }
}