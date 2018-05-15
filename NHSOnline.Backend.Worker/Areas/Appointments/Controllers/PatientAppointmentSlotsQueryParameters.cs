using System;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Controllers
{
    public class PatientAppointmentSlotsQueryParameters
    {
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
    }
}
