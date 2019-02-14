using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHSOnline.Backend.GpSystems.SharedModels;

namespace NHSOnline.Backend.GpSystems.Appointments.Models
{
    public class AppointmentSlotsResponse
    {
        public string BookingGuidance { get; set; } = string.Empty;

        [JsonConverter(typeof(StringEnumConverter), false)]
        public Necessity BookingReasonNecessity { get; set; } = Necessity.Mandatory;

        public IEnumerable<Slot> Slots { get; set; } = Array.Empty<Slot>();
        public IEnumerable<PatientTelephoneNumber> TelephoneNumbers { get; set; } = Array.Empty<PatientTelephoneNumber>();
    }
}
