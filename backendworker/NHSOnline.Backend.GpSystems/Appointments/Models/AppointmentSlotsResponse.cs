using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Appointments.Models
{
    public class AppointmentSlotsResponse
    {
        public string BookingGuidance { get; set; } = string.Empty;

        [JsonConverter(typeof(StringEnumConverter), false)]
        public Necessity BookingReasonNecessity { get; set; } = Necessity.Mandatory;

        public int BookingReasonCharacterLimit { get; set; } = Constants.BookingReasonCharacterLimit.FrontendLimit;

        public IList<Slot> Slots { get; set; } = new List<Slot>();
        public IList<PatientTelephoneNumber> TelephoneNumbers { get; set; } = new List<PatientTelephoneNumber>();
    }
}
