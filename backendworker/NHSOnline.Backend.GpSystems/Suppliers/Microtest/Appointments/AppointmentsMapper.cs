using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class AppointmentsMapper : IAppointmentsMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<AppointmentsMapper> _logger;

        public AppointmentsMapper(IDateTimeOffsetProvider dateTimeOffsetProvider, ILogger<AppointmentsMapper> logger)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
        }

        public IEnumerable<Appointment> Map(
            IEnumerable<Suppliers.Microtest.Models.Appointments.Appointment> sourceAppointments)
        {
            _logger.LogEnter();

            var appointments = new List<Appointment>();

            if (sourceAppointments == null || !sourceAppointments.Any())
            {
                return appointments;
            }

            var now = new DateTimeOffset(DateTime.Now);

            foreach (var sourceAppointment in sourceAppointments)
            {
                var startTime = ParseSlotTime(sourceAppointment.StartTime, "Start");

                if (startTime == null)
                {
                    continue;
                }

                if (startTime < now)
                {
                    var resultAppointment = new PastAppointment();
                    MapAppointment(sourceAppointment, resultAppointment, startTime);
                    appointments.Add(resultAppointment);
                }
                else
                {
                    var resultAppointment = new UpcomingAppointment();
                    MapAppointment(sourceAppointment, resultAppointment, startTime);
                    appointments.Add(resultAppointment);
                }
            }

            return appointments;
        }

        private void MapAppointment(Suppliers.Microtest.Models.Appointments.Appointment sourceAppointment,
            Appointment resultAppointment, DateTimeOffset? startTime)
        {
            resultAppointment.Id = sourceAppointment.Id;
            resultAppointment.StartTime = startTime.Value;
            resultAppointment.EndTime = ParseSlotTime(sourceAppointment.EndTime, "End");
            resultAppointment.Clinicians = sourceAppointment.Clinicians;
            resultAppointment.Location = string.IsNullOrWhiteSpace(sourceAppointment.Location)
                ? string.Empty
                : sourceAppointment.Location;
            resultAppointment.Type = string.IsNullOrWhiteSpace(sourceAppointment.Type)
                ? string.Empty
                : sourceAppointment.Type;
            resultAppointment.SessionName = string.Empty;
        }

        private DateTimeOffset? ParseSlotTime(string time, string usage)
        {
            var success = _dateTimeOffsetProvider.TryCreateDateTimeOffset(time, out var parsedTime);
            if (!success)
            {
                _logger.LogError($"Unable to parse Microtest Appointment {usage} Time of '{time}'");
            }

            return parsedTime;
        }
    }
}