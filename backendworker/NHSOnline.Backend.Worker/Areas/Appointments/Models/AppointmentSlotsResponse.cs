using System;
using System.Collections.Generic;
using NHSOnline.Backend.Worker.Areas.SharedModels;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class AppointmentSlotsResponse
    {
        public string BookingGuidance { get; set; } = string.Empty;

        public Necessity BookingReasonNecessity { get; set; } = Necessity.Mandatory;

        public IEnumerable<Slot> Slots { get; set; } = Array.Empty<Slot>();
    }
}
