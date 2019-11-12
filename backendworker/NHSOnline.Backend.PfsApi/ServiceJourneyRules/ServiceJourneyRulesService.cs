using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.ServiceJourneyRules.Common;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public class ServiceJourneyRulesService: IServiceJourneyRulesService
    {
        private readonly IServiceJourneyRulesClient _serviceJourneyRulesClient;
        private readonly ILogger<ServiceJourneyRulesService> _logger;

        public ServiceJourneyRulesService(IServiceJourneyRulesClient serviceJourneyRulesClient,  ILogger<ServiceJourneyRulesService> logger)
        {
            _serviceJourneyRulesClient = serviceJourneyRulesClient;
            _logger = logger;
        }

        public async Task<ServiceJourneyRulesConfigResult> GetServiceJourneyRulesForOdsCode(string odsCode)
        {
            return await GetServiceJourneyRules(odsCode);
        }

        public async Task<ServiceJourneyRulesConfigResult> GetServiceJourneyRulesForLinkedAccount(string odsCode)
        {
            return await GetServiceJourneyRules(odsCode, journeys =>
            {
                UpdateJourneyRulesForLinkedAccount(journeys);
                _logger.LogInformation($"Overriding SJR journeys for {odsCode} while in proxy mode");
            });
        }

        private async Task<ServiceJourneyRulesConfigResult> GetServiceJourneyRules(string odsCode, Action<Journeys> journeyOverrides = null)
        {
            _logger.LogEnter();
            try
            {
                var result = await _serviceJourneyRulesClient.GetServiceJourneyRules(odsCode);

                if (result.HasSuccessResponse && result.Body?.Journeys != null)
                {
                    if (journeyOverrides != null)
                    {
                        journeyOverrides(result.Body.Journeys);
                    }

                    return new ServiceJourneyRulesConfigResult.Success(result.Body);
                }

                _logger.LogError($"Failed to retrieve Service Journey Rules for ods code: {odsCode}");
                return new ServiceJourneyRulesConfigResult.NotFound();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Exception calling Service Journey Rules API, {ex}");
                return new ServiceJourneyRulesConfigResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private static void UpdateJourneyRulesForLinkedAccount(Journeys journeys)
        {
            // These operations are never allowed when proxying
            journeys.NominatedPharmacy = false;
            journeys.Messaging = false;
            journeys.Notifications = false;

            // Set Appointment journey provider
            switch (journeys.Appointments.Provider)
            {
                case AppointmentsProvider.gpAtHand:
                case AppointmentsProvider.informatica:
                    journeys.Appointments.Provider = AppointmentsProvider.linkedAccount;
                    break;
                default:
                    journeys.Appointments.Provider = AppointmentsProvider.im1;
                    break;
            }
        }
    }
}