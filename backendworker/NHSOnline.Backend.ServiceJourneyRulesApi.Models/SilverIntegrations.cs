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
        public IList<SecondaryAppointmentProvider> SecondaryAppointments { get; set; }

        public SilverIntegrations Clone()
        {
            var silverIntegrations = MemberwiseClone() as SilverIntegrations;

            silverIntegrations.CarePlans = CarePlans.ToList();
            silverIntegrations.Consultations = Consultations.ToList();
            silverIntegrations.HealthTrackers = HealthTrackers.ToList();
            silverIntegrations.Libraries = Libraries.ToList();
            silverIntegrations.Medicines = Medicines.ToList();
            silverIntegrations.Messages = Messages.ToList();
            silverIntegrations.SecondaryAppointments = SecondaryAppointments.ToList();

            return silverIntegrations;
        }
    }
}