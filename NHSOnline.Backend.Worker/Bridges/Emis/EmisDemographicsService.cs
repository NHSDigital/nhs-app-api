using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Bridges.Emis.Demographics;
using NHSOnline.Backend.Worker.Router.Demographics;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisDemographicsService : IDemographicsService
    {
        private readonly ILogger _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisDemographicsMapper _emisDemographicsMapper;

        public EmisDemographicsService(
            ILoggerFactory loggerFactory, 
            IEmisClient emisClient,
            IEmisDemographicsMapper emisDemographicsMapper)
        {
            _logger = loggerFactory.CreateLogger<EmisCourseService>();
            _emisClient = emisClient;
            _emisDemographicsMapper = emisDemographicsMapper;
        }

        public async Task<GetMyRecordResult> Get(UserSession userSession)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var demographicsResponse = await _emisClient.DemographicsGet(emisUserSession.UserPatientLinkToken, emisUserSession.SessionId, emisUserSession.EndUserSessionId);

                if (!demographicsResponse.HasSuccessStatusCode)
                {
                    _logger.LogError($"Unsuccessful request retrieving demographics. Status code: {(int)demographicsResponse.StatusCode}");
                    return new GetMyRecordResult.Unsuccessful();
                }
                
                var result = _emisDemographicsMapper.Map(demographicsResponse.Body);

                return new GetMyRecordResult.SuccessfullyRetrieved(result);

            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving courses");
                return new GetMyRecordResult.Unsuccessful();
            }
        }
    }
}
