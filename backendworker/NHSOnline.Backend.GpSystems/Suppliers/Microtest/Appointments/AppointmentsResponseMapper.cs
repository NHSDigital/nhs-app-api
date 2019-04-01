using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public interface IAppointmentsResponseMapper
    {
        AppointmentsResponse Map(AppointmentsGetResponse appointmentsResponse);
    }
    public class AppointmentsResponseMapper : IAppointmentsResponseMapper
    {
        private readonly ILogger<AppointmentsResponseMapper> _logger;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public AppointmentsResponseMapper(ILogger<AppointmentsResponseMapper> logger, IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _logger = logger;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public AppointmentsResponse Map(AppointmentsGetResponse appointmentsResponse)
        {
            _logger.LogEnter();

            var appointments = new List<UpcomingAppointment>();

            var now = _dateTimeOffsetProvider.CreateDateTimeOffset();

            foreach (var sourceAppointment in appointmentsResponse.Appointments)
            {
                if (sourceAppointment.StartTime == null || sourceAppointment.StartTime < now)
                {
                    continue;
                }
                
                var resultAppointment = new UpcomingAppointment
                {
                    Id = sourceAppointment.Id,
                    StartTime = sourceAppointment.StartTime.Value,
                    EndTime = sourceAppointment.EndTime,
                    Clinicians = sourceAppointment.Clinicians,
                    Location = sourceAppointment.Location,
                    Type = sourceAppointment.Type,
                    SessionName = string.Empty
                };

                appointments.Add(resultAppointment);
            }
            
            _logger.LogExit();
            
            return new AppointmentsResponse
            {
                UpcomingAppointments = appointments
            };
        }
    }
}
