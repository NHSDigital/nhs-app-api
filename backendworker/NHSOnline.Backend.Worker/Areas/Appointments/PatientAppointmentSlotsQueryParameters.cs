using System;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class PatientAppointmentSlotsQueryParameters
    {
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
    }
}
