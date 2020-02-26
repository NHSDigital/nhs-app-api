using System.Linq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests
{
    public class SilverIntegrationsBuilder
    {
        private readonly SilverIntegrations _silverIntegrations = new SilverIntegrations();

        public SilverIntegrationsBuilder SecondaryAppointments(params SecondaryAppointmentProvider[] providers)
        {
            _silverIntegrations.SecondaryAppointments = providers.ToList();
            return this;
        }

        public SilverIntegrationsBuilder Messages(params MessagesProvider[] providers)
        {
            _silverIntegrations.Messages = providers.ToList();
            return this;
        }

        public SilverIntegrationsBuilder Consultations(params ConsultationsProvider[] providers)
        {
            _silverIntegrations.Consultations = providers.ToList();
            return this;
        }

        public SilverIntegrations Build()
        {
            return _silverIntegrations;
        }
    }
}