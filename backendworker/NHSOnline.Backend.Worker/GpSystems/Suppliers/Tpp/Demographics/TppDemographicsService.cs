using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Demographics;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Demographics
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

        public async Task<GetDemographicsResult> GetDemographics(UserSession userSession)
        {
            var tppUserSession = (TppUserSession) userSession;

            try
            {               
                var demographicsResponse = await _tppClient.PatientSelectedPost(tppUserSession);

                if (!demographicsResponse.HasSuccessResponse)
                {
                    if (demographicsResponse.HasForbiddenResponse)
                    {
                        _logger.LogWarning("User does not have access to their patient record for Tpp");
                        return new GetDemographicsResult.UserHasNoAccess();
                    }

                    _logger.LogError($"Unsuccessful request retrieving patient selected information for Tpp. Status code: {(int)demographicsResponse.StatusCode}");
                    return new GetDemographicsResult.Unsuccessful();
                }
                
                var result = _tppDemographicsMapper.Map(demographicsResponse.Body);

                return new GetDemographicsResult.SuccessfullyRetrieved(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving patient selected information for Tpp");
                return new GetDemographicsResult.Unsuccessful();
            }
        }
    }
}
