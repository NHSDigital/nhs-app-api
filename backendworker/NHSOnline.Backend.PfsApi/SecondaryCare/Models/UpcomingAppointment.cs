extern alias r4;

using System;
using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Utility;
using Newtonsoft.Json;
using FhirAppointmentStatus = r4::Hl7.Fhir.Model.Appointment.AppointmentStatus;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class UpcomingAppointment : SecondaryCareSummaryItem
    {
        public override string ItemType => SummaryItemType.UpcomingAppointment.ToString();

        public DateTimeOffset? AppointmentDateTime { get; set; }

        public string AppointmentStatus { get; set; }

        public string LocationDescription { get; set; }

        public string Provider { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Intentional; we wish to expose this as a string, do not intend to parse the URL")]
        public string DeepLinkUrl { get; set; }

        [JsonIgnore]
        public bool IsConfirmed => AppointmentDateTime != null;

        [JsonIgnore]
        public bool IsCancelled => string.Equals(FhirAppointmentStatus.Cancelled.GetLiteral(), AppointmentStatus, StringComparison.Ordinal);
    }
}