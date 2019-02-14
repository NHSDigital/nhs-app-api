using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments
{
    public class AppointmentSlotsGetResponse
    {
        public IEnumerable<Slot> Slots { get; set; } = Array.Empty<Slot>();
    }
}
