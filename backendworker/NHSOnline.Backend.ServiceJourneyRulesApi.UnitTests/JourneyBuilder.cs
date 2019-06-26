using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests
{
    internal static class JourneyBuilder
    {
        public static Journeys Build(
            AppointmentsProvider? appointmentsProvider = null,
            CdssProvider? cdssAdviceProvider = null,
            CdssProvider? cdssAdminProvider = null,
            string informaticaUrl = null,
            string cdssAdviceServiceDefinition = null,
            string cdssAdminServiceDefinition = null)
        {
            var journeys = new Journeys();

            if (appointmentsProvider.HasValue)
            {
                journeys.Appointments = new Appointments { Provider = appointmentsProvider.Value };
            }
            else if (!string.IsNullOrWhiteSpace(informaticaUrl))
            {
                journeys.Appointments = new Appointments
                {
                    Provider = AppointmentsProvider.informatica,
                    InformaticaUrl = informaticaUrl
                };
            }
            
            if (cdssAdviceProvider.HasValue)
            {
                journeys.CdssAdvice = new Cdss
                {
                    Provider = cdssAdviceProvider.Value,
                    ServiceDefinition = cdssAdviceServiceDefinition
                };
            }

            if (cdssAdminProvider.HasValue)
            {
                journeys.CdssAdmin = new Cdss
                {
                    Provider = cdssAdminProvider.Value,
                    ServiceDefinition = cdssAdminServiceDefinition
                };
            }

            return journeys;
        }
    }
}