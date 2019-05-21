using System;
using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public class AppointmentMapper : IAppointmentMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public AppointmentMapper(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public IEnumerable<Appointment> Map(List<Models.Appointments.Appointment> sourceAppointments)
        {
            if (sourceAppointments == null)
            {
                yield break;
            }
            
            var now = _dateTimeOffsetProvider.CreateDateTimeOffset();

            foreach (var sourceAppointment in sourceAppointments)
            {
                var startTimeSuccess = _dateTimeOffsetProvider.TryCreateDateTimeOffset(sourceAppointment.StartDate?.TrimEnd('Z'),
                    out var startTime);

                if (!startTimeSuccess)
                    continue;

                _dateTimeOffsetProvider.TryCreateDateTimeOffset(sourceAppointment.EndDate?.TrimEnd('Z'), out var endTime);

                var appointment = now >= startTime
                    ? (Appointment) new PastAppointment()
                    : new UpcomingAppointment();

                appointment.Id = sourceAppointment.ApptId;
                appointment.StartTime = startTime.GetValueOrDefault();
                appointment.EndTime = endTime;
                appointment.Location = sourceAppointment.SiteName;
                appointment.Type = sourceAppointment.Details;
                appointment.SessionName = string.Empty;
                appointment.Clinicians = Array.Empty<string>();

                yield return appointment;
            }
        }
    }
}
