using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Demographics
{
    internal class TppDemographicsService : IDemographicsService
    {
        private readonly ILogger<TppDemographicsService> _logger;
        private readonly ITppClientRequest<TppUserSession, PatientSelectedReply> _patientSelected;
        private readonly ITppDemographicsMapper _tppDemographicsMapper;

        public TppDemographicsService(
            ILoggerFactory loggerFactory,
            ITppClientRequest<TppUserSession, PatientSelectedReply> patientSelected,
            ITppDemographicsMapper tppDemographicsMapper)
        {
            _logger = loggerFactory.CreateLogger<TppDemographicsService>();
            _patientSelected = patientSelected;
            _tppDemographicsMapper = tppDemographicsMapper;
        }

        public async Task<DemographicsResult> GetDemographics(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var tppUserSession = (TppUserSession)gpLinkedAccountModel.GpUserSession;

            try
            {
                _logger.LogEnter();
                var demographicsResponse = await _patientSelected.Post(tppUserSession);

                if (!demographicsResponse.HasSuccessResponse)
                {
                    if (demographicsResponse.HasForbiddenResponse)
                    {
                        _logger.LogWarning("User does not have access to their patient record for Tpp");
                        return new DemographicsResult.Forbidden();
                    }
                    _logger.LogError($"Unsuccessful request retrieving patient selected information for Tpp. Status code: {(int)demographicsResponse.StatusCode}");
                    return new DemographicsResult.BadGateway();
                }

                var result = _tppDemographicsMapper.Map(demographicsResponse.Body);

                return new DemographicsResult.Success(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving patient selected information for Tpp");
                return new DemographicsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}