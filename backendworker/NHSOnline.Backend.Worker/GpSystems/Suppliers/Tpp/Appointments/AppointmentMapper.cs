using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using System.Collections.Generic;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public interface IAppointmentMapper
    {
        IEnumerable<UpcomingAppointment> Map(List<Models.Appointments.Appointment> appointments);
    }
    public class AppointmentMapper : IAppointmentMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public AppointmentMapper(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public IEnumerable<UpcomingAppointment> Map(List<Models.Appointments.Appointment> appointments)
        {
            if (appointments == null)
            {
                yield break;
            }

            var now = _dateTimeOffsetProvider.CreateDateTimeOffset();

            foreach (var appointment in appointments)
            {
                var startTimeSuccess = _dateTimeOffsetProvider.TryCreateDateTimeOffset(appointment.StartDate?.TrimEnd('Z'),
                    out var startTime);

                if (!startTimeSuccess || now > startTime)
                    continue;

                _dateTimeOffsetProvider.TryCreateDateTimeOffset(appointment.EndDate?.TrimEnd('Z'), out var endTime);

                yield return new UpcomingAppointment
                {
                    Id = appointment.ApptId,
                    StartTime = startTime.GetValueOrDefault(),
                    EndTime = endTime,
                    Location = appointment.SiteName,
                    Type = appointment.Details
                };
            }
        }
    }
}
