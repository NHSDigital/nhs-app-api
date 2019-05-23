using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Demographics
{
    public class VisionDemographicsService : IDemographicsService
    {
        private readonly ILogger<VisionDemographicsService> _logger;
        private readonly IVisionClient _visionClient;
        private readonly IVisionDemographicsMapper _visionDemographicsMapper;

        public VisionDemographicsService(
            ILoggerFactory loggerFactory, 
            IVisionClient visionClient,
            IVisionDemographicsMapper visionDemographicsMapper)
        {
            _logger = loggerFactory.CreateLogger<VisionDemographicsService>();
            _visionClient = visionClient;
            _visionDemographicsMapper = visionDemographicsMapper;
        }

        public async Task<DemographicsResult> GetDemographics(GpUserSession gpUserSession)
        {
            _logger.LogEnter();
            var visionUserSession = (VisionUserSession)gpUserSession;

            try
            {
                var demographicsRequestContent =
                    new DemographicsRequest { PatientId = visionUserSession.PatientId };

                var demographicsResponse = await _visionClient.GetDemographics(visionUserSession, demographicsRequestContent);

                try
                {
                    if (demographicsResponse.HasErrorResponse)
                    {
                        if (demographicsResponse.IsAccessDeniedError)
                        {
                            _logger.LogWarning("User does not have access to their patient record");
                            return new DemographicsResult.Forbidden();
                        }

                        _logger.LogError(
                            $"Unsuccessful request retrieving demographics information for Vision. Status code: {(int) demographicsResponse.StatusCode}");
                        _logger.LogVisionErrorResponse(demographicsResponse);
                        return new DemographicsResult.BadGateway();  
                    }

                    var result = _visionDemographicsMapper.Map(demographicsResponse.Body.Demographics, visionUserSession.NhsNumber);
                    
                    return new DemographicsResult.Success(result);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Something went wrong building the Vision Demographics response");
                    return new DemographicsResult.InternalServerError();
                }  
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving patient selected information for Vision");
                return new DemographicsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
