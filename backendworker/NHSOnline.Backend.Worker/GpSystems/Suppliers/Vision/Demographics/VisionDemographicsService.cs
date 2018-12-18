using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Demographics
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

        public async Task<DemographicsResult> GetDemographics(UserSession userSession)
        {
            _logger.LogEnter();
            var visionUserSession = (VisionUserSession) userSession.GpUserSession;

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
                            return new DemographicsResult.UserHasNoAccess();
                        }

                        _logger.LogError(
                            $"Unsuccessful request retrieving demographics information for Vision. Status code: {(int) demographicsResponse.StatusCode}");
                        return new DemographicsResult.Unsuccessful();  
                    }

                    var result = _visionDemographicsMapper.Map(demographicsResponse.Body.Demographics, visionUserSession.NhsNumber);
                    
                    return new DemographicsResult.SuccessfullyRetrieved(result);
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
                return new DemographicsResult.Unsuccessful();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}