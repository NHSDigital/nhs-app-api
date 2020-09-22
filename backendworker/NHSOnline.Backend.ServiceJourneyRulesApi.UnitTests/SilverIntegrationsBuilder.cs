using System.Linq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests
{
    public class SilverIntegrationsBuilder
    {
        private readonly SilverIntegrations _silverIntegrations = new SilverIntegrations();

        public SilverIntegrationsBuilder CarePlans(params CarePlansProvider[] providers)
        {
            _silverIntegrations.CarePlans = providers.ToList();
            return this;
        }

        public SilverIntegrationsBuilder Consultations(params ConsultationsProvider[] providers)
        {
            _silverIntegrations.Consultations = providers.ToList();
            return this;
        }

        public SilverIntegrationsBuilder ConsultationsAdmin(params ConsultationsAdminProvider[] providers)
        {
            _silverIntegrations.ConsultationsAdmin = providers.ToList();
            return this;
        }

        public SilverIntegrationsBuilder HealthTrackers(params HealthTrackersProvider[] providers)
        {
            _silverIntegrations.HealthTrackers = providers.ToList();
            return this;
        }

        public SilverIntegrationsBuilder Libraries(params LibrariesProvider[] providers)
        {
            _silverIntegrations.Libraries = providers.ToList();
            return this;
        }

        public SilverIntegrationsBuilder Medicines(params MedicinesProvider[] providers)
        {
            _silverIntegrations.Medicines = providers.ToList();
            return this;
        }

        public SilverIntegrationsBuilder Messages(params MessagesProvider[] providers)
        {
            _silverIntegrations.Messages = providers.ToList();
            return this;
        }

        public SilverIntegrationsBuilder SecondaryAppointments(params SecondaryAppointmentsProvider[] providers)
        {
            _silverIntegrations.SecondaryAppointments = providers.ToList();
            return this;
        }

        public SilverIntegrationsBuilder TestResults(params TestResultsProvider[] providers)
        {
            _silverIntegrations.TestResults = providers.ToList();
            return this;
        }

        public SilverIntegrations Build()
        {
            return _silverIntegrations;
        }
    }
}