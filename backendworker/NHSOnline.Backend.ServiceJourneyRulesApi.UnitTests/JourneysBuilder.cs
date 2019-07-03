using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests
{
    internal class JourneysBuilder
    {
        private readonly Journeys _journeys = new Journeys();

        public JourneysBuilder AppointmentProvider(AppointmentsProvider? provider, string informaticaUrl = null)
        {
            if (provider != null)
            {
                _journeys.Appointments = new Appointments
                {
                    Provider = provider.Value,
                    InformaticaUrl = informaticaUrl
                };
            }
            else
            {
                _journeys.Appointments = null;
            }
            return this;
        }

        public JourneysBuilder CdssAdviceProvider(CdssProvider? cdssAdviceProvider,
            string cdssAdviceServiceDefinition = null)
        {
            if (cdssAdviceProvider != null)
            {
                _journeys.CdssAdvice = new Cdss
                {
                    Provider = cdssAdviceProvider.Value,
                    ServiceDefinition = cdssAdviceServiceDefinition
                };
            }
            else
            {
                _journeys.CdssAdvice = null;
            }
            return this;
        }

        public JourneysBuilder CdssAdminProvider(CdssProvider? cdssAdminProvider,
            string cdssAdminServiceDefinition = null)
        {
            if (cdssAdminProvider != null)
            {
                _journeys.CdssAdmin = new Cdss
                {
                    Provider = cdssAdminProvider.Value,
                    ServiceDefinition = cdssAdminServiceDefinition
                };
            }
            else
            {
                _journeys.CdssAdmin = null;
            }
            return this;
        }

        public JourneysBuilder MedicalRecord(MedicalRecordProvider? provider)
        {
            if (provider != null)
            {
                _journeys.MedicalRecord = new MedicalRecord
                {
                    Provider = provider.Value
                };
            }
            else
            {
                _journeys.MedicalRecord = null;
            }
            return this;
        }

        public JourneysBuilder Prescriptions(PrescriptionsProvider? provider)
        {
            if (provider != null)
            {
                _journeys.Prescriptions = new Prescriptions
                {
                    Provider = provider.Value
                };
            }
            else
            {
                _journeys.Prescriptions = null;
            }
            return this;
        }

        public JourneysBuilder NominatedPharmacyEnabled(bool? enabled)
        {
            _journeys.NominatedPharmacy = enabled;
            return this;
        }

        public Journeys Build()
        {
            return _journeys;
        }
    }
}