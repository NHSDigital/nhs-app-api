using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models.Attributes;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class SilverIntegrations : ICloneable<SilverIntegrations>
    {
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<AccountAdminProvider> AccountAdmin { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<AppointmentBookingsProvider> AppointmentBookings { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<CarePlansProvider> CarePlans { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<ConsultationsProvider> Consultations { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<ConsultationsAdminProvider> ConsultationsAdmin { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<HealthTrackersProvider> HealthTrackers { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<LibrariesProvider> Libraries { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<MedicinesProvider> Medicines { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<MessagesProvider> Messages { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<ParticipationProvider> Participation { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<RecordSharingProvider> RecordSharing { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<SecondaryAppointmentsProvider> SecondaryAppointments { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<TestResultsProvider> TestResults { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<VaccineRecordProvider> VaccineRecord { get; set; }

        public SilverIntegrations Clone()
        {
            var silverIntegrations = new SilverIntegrations
            {
                AccountAdmin = Clone(AccountAdmin),
                AppointmentBookings = Clone(AppointmentBookings),
                CarePlans = Clone(CarePlans),
                Consultations = Clone(Consultations),
                ConsultationsAdmin = Clone(ConsultationsAdmin),
                HealthTrackers = Clone(HealthTrackers),
                Libraries = Clone(Libraries),
                Medicines = Clone(Medicines),
                Messages = Clone(Messages),
                Participation = Clone(Participation),
                RecordSharing = Clone(RecordSharing),
                SecondaryAppointments = Clone(SecondaryAppointments),
                TestResults = Clone(TestResults),
                VaccineRecord = Clone(VaccineRecord)
            };

            return silverIntegrations;
        }

        public void Merge(SilverIntegrations other)
        {
            AccountAdmin = Merge(AccountAdmin, other?.AccountAdmin);
            AppointmentBookings = Merge(AppointmentBookings, other?.AppointmentBookings);
            CarePlans = Merge(CarePlans, other?.CarePlans);
            Consultations = Merge(Consultations, other?.Consultations);
            ConsultationsAdmin = Merge(ConsultationsAdmin, other?.ConsultationsAdmin);
            HealthTrackers = Merge(HealthTrackers, other?.HealthTrackers);
            Libraries = Merge(Libraries, other?.Libraries);
            Medicines = Merge(Medicines, other?.Medicines);
            Messages = Merge(Messages, other?.Messages);
            Participation = Merge(Participation, other?.Participation);
            RecordSharing = Merge(RecordSharing, other?.RecordSharing);
            SecondaryAppointments = Merge(SecondaryAppointments, other?.SecondaryAppointments);
            TestResults = Merge(TestResults, other?.TestResults);
            VaccineRecord = Merge(VaccineRecord, other?.VaccineRecord);
        }

        private IList<T> Clone<T>(IList<T> toClone) where T : Enum
        {
            return toClone?.ToList();
        }

        private IList<T> Merge<T>(IList<T> current, IList<T> toMerge) where T : Enum
        {
            if (toMerge is null)
            {
                return current;
            }
            current ??= new List<T>();
            return Purge(current.Union(toMerge));
        }

        private static IList<T> Purge<T>(IEnumerable<T> values) where T : Enum
        {
            var output = values.ToList();

            var removeAttributes = output
                .Where(x => x.HasAttribute<RemovesSilverIntegrationAttribute>())
                .Select(x => x.GetAttribute<RemovesSilverIntegrationAttribute>())
                .ToList();

            foreach (var attribute in removeAttributes)
            {
                output.RemoveAll(x => x.ToString() == attribute.AttributeToRemove);
            }

            if (removeAttributes.Any())
            {
                output.RemoveAll(x => x.HasAttribute<RemovesSilverIntegrationAttribute>());
            }

            return output;
        }
    }
}
