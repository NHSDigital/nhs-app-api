using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class SilverIntegrations : ICloneable<SilverIntegrations>
    {
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<CarePlansProvider> CarePlans { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<ConsultationsProvider> Consultations { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<HealthTrackersProvider> HealthTrackers { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<LibrariesProvider> Libraries { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<MedicinesProvider> Medicines { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<MessagesProvider> Messages { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<SecondaryAppointmentsProvider> SecondaryAppointments { get; set; }

        public SilverIntegrations Clone()
        {
            var silverIntegrations = new SilverIntegrations
            {
                CarePlans = CarePlans?.ToList(),
                Consultations = Consultations?.ToList(),
                HealthTrackers = HealthTrackers?.ToList(),
                Libraries = Libraries?.ToList(),
                Medicines = Medicines?.ToList(),
                Messages = Messages?.ToList(),
                SecondaryAppointments = SecondaryAppointments?.ToList()
            };

            return silverIntegrations;
        }

        public void Merge(SilverIntegrations other)
        {
            if (other?.CarePlans != null)
            {
                CarePlans ??= new List<CarePlansProvider>();
                CarePlans = CarePlans.Union(other.CarePlans).ToList();
            }

            if (other?.Consultations != null)
            {
                Consultations ??= new List<ConsultationsProvider>();
                Consultations = Consultations.Union(other.Consultations).ToList();
            }

            if (other?.HealthTrackers != null)
            {
                HealthTrackers ??= new List<HealthTrackersProvider>();
                HealthTrackers = HealthTrackers.Union(other.HealthTrackers).ToList();
            }

            if (other?.Libraries != null)
            {
                Libraries ??= new List<LibrariesProvider>();
                Libraries = Libraries.Union(other.Libraries).ToList();
            }

            if (other?.Medicines != null)
            {
                Medicines ??= new List<MedicinesProvider>();
                Medicines = Medicines.Union(other.Medicines).ToList();
            }

            if (other?.Messages != null)
            {
                Messages ??= new List<MessagesProvider>();
                Messages = Messages.Union(other.Messages).ToList();
            }

            if (other?.SecondaryAppointments != null)
            {
                SecondaryAppointments ??= new List<SecondaryAppointmentsProvider>();
                SecondaryAppointments = SecondaryAppointments.Union(other.SecondaryAppointments).ToList();
            }
        }
    }
}