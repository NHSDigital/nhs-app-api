using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class AppointmentsResponseMapper : IAppointmentsResponseMapper
    {
        private readonly ILogger<AppointmentsResponseMapper> _logger;
        private readonly ICancellationReasonService _defaultCancellationReasons;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        
        public AppointmentsResponseMapper(
            ILogger<AppointmentsResponseMapper> logger,
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            ICancellationReasonService defaultCancellationReasons)
        {
            _logger = logger;
            _defaultCancellationReasons = defaultCancellationReasons;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public AppointmentsResponse Map(AppointmentsGetResponse appointmentsGetResponse)
        {
            _logger.LogEnter();

            var appointments = new List<UpcomingAppointment>();
            
            var now = _dateTimeOffsetProvider.CreateDateTimeOffset();

            foreach (var sourceAppointment in appointmentsGetResponse.Appointments)
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

            var cancellationReasons = _defaultCancellationReasons.GetDefaultCancellationReasons();
            
            _logger.LogExit();
            
            return new AppointmentsResponse
            {
                UpcomingAppointments = appointments,
                CancellationReasons = cancellationReasons
            };
        }
    }
}