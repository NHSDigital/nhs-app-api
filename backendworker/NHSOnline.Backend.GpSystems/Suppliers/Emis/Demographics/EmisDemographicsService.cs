using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Demographics
{
    public class EmisDemographicsService : IDemographicsService, IEmisDemographicsService
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

        public async Task<DemographicsResult> GetDemographics(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            try
            {
                _logger.LogInformation($"Trying to find UserPatientLinkToken using Id {gpLinkedAccountModel.PatientId}");
                var emisHttpRequestData = gpLinkedAccountModel.BuildEmisRequestParameters(_logger);

                var demographicsResponse = await _emisClient.DemographicsGet(emisHttpRequestData);

                if (!demographicsResponse.HasSuccessResponse)
                {
                    // User does not have access
                    if (demographicsResponse.HasForbiddenResponse())
                    {
                        _logger.LogWarning("User does not have access to their patient record");
                        _logger.LogEmisErrorResponse(demographicsResponse);
                        return new DemographicsResult.Forbidden();
                    }

                    _logger.LogError($"Unsuccessful request retrieving demographics. Status code: {(int)demographicsResponse.StatusCode}");
                    _logger.LogEmisErrorResponse(demographicsResponse);
                    return new DemographicsResult.BadGateway();
                }

                _logger.LogInformation("Mapping EMIS responses to universal DemographicsResponse class instance");
                var result = _emisDemographicsMapper.Map(demographicsResponse.Body);

                return new DemographicsResult.Success(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving demographics");
                return new DemographicsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
