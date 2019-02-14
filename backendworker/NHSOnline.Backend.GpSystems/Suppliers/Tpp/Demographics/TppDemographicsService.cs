using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Demographics
{
    public class TppDemographicsService : IDemographicsService
    {
        private readonly ILogger<TppDemographicsService> _logger;
        private readonly ITppClient _tppClient;
        private readonly ITppDemographicsMapper _tppDemographicsMapper;

        public TppDemographicsService(
            ILoggerFactory loggerFactory, 
            ITppClient tppClient,
            ITppDemographicsMapper tppDemographicsMapper)
        {
            _logger = loggerFactory.CreateLogger<TppDemographicsService>();
            _tppClient = tppClient;
            _tppDemographicsMapper = tppDemographicsMapper;
        }

        public async Task<DemographicsResult> GetDemographics(GpUserSession gpUserSession)
        {
            var tppUserSession = (TppUserSession)gpUserSession;

            try
            {
                _logger.LogEnter();
                var demographicsResponse = await _tppClient.PatientSelectedPost(tppUserSession);

                if (!demographicsResponse.HasSuccessResponse)
                {
                    if (demographicsResponse.HasForbiddenResponse)
                    {
                        _logger.LogWarning("User does not have access to their patient record for Tpp");
                        return new DemographicsResult.UserHasNoAccess();
                    }
                    _logger.LogError($"Unsuccessful request retrieving patient selected information for Tpp. Status code: {(int)demographicsResponse.StatusCode}");
                    return new DemographicsResult.Unsuccessful();
                }
                
                var result = _tppDemographicsMapper.Map(demographicsResponse.Body);
                
                return new DemographicsResult.SuccessfullyRetrieved(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving patient selected information for Tpp");
                return new DemographicsResult.Unsuccessful();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
