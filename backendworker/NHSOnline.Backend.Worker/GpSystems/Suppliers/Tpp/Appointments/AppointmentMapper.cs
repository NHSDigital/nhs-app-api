using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public interface IAppointmentMapper
    {
        IEnumerable<Appointment> Map(List<Models.Appointments.Appointment> appointments);
    }
    public class AppointmentMapper : IAppointmentMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public AppointmentMapper(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public IEnumerable<Appointment> Map(List<Models.Appointments.Appointment> appointments)
        {
            if (appointments == null)
                yield break;

            var now = _dateTimeOffsetProvider.CreateDateTimeOffset(DateTime.Now);

            foreach (var appointment in appointments)
            {
                DateTimeOffset startTime;
                DateTimeOffset? endTime;

                try
                {
                    startTime = _dateTimeOffsetProvider.CreateDateTimeOffset(appointment.StartDate.TrimEnd('Z'));
                }
                catch
                {
                    continue;
                }

                if (now > startTime)
                    continue;

                try
                {
                    endTime = _dateTimeOffsetProvider.CreateDateTimeOffset(appointment.EndDate.TrimEnd('Z'));
                }
                catch
                {
                    endTime=null;
                }
                
                yield return new Appointment
                {
                    Id = appointment.ApptId,
                    StartTime = startTime,
                    EndTime = endTime,
                    Location = appointment.SiteName,
                    Type = appointment.Details
                };
            }
        }
    }
}
