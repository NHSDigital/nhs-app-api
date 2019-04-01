using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments
{
    public class AppointmentsGetResponse
    {
        public IEnumerable<Appointment> Appointments { get; set; } = Array.Empty<Appointment>();
    }
}
