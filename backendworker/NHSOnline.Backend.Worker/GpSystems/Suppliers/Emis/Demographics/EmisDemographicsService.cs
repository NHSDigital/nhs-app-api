using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Demographics;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Demographics
{
    public class EmisDemographicsService : IDemographicsService
    {
        private readonly ILogger<EmisDemographicsService> _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisDemographicsMapper _emisDemographicsMapper;

        public EmisDemographicsService(
            ILoggerFactory loggerFactory, 
            IEmisClient emisClient,
            IEmisDemographicsMapper emisDemographicsMapper)
        {
            _logger = loggerFactory.CreateLogger<EmisDemographicsService>();
            _emisClient = emisClient;
            _emisDemographicsMapper = emisDemographicsMapper;
        }

        public async Task<GetDemographicsResult> Get(UserSession userSession)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var demographicsResponse = await _emisClient.DemographicsGet(emisUserSession.UserPatientLinkToken, emisUserSession.SessionId, emisUserSession.EndUserSessionId);

                if (!demographicsResponse.HasSuccessStatusCode)
                {
                    // User does not have access
                    if (demographicsResponse.HasForbiddenResponse())
                    {
                        _logger.LogWarning("User does not have access to their patient record");
                        return new GetDemographicsResult.UserHasNoAccess();
                    }

                    _logger.LogError($"Unsuccessful request retrieving demographics. Status code: {(int)demographicsResponse.StatusCode}");
                    return new GetDemographicsResult.Unsuccessful();
                }
                
                var result = _emisDemographicsMapper.Map(demographicsResponse.Body);

                return new GetDemographicsResult.SuccessfullyRetrieved(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving courses");
                return new GetDemographicsResult.Unsuccessful();
            }
        }
    }
}
