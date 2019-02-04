using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Temporal;
using Appointment = NHSOnline.Backend.Worker.Areas.Appointments.Models.Appointment;
using Location = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public interface IAppointmentsMapper
    {
        IEnumerable<Appointment> Map(
            IEnumerable<Models.Appointment> sourceAppointments, 
            IEnumerable<Location> locations, 
            IEnumerable<SessionHolder> sessionHolders, 
            IEnumerable<Models.Session> sessions);
    }

    public class AppointmentsMapper : IAppointmentsMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<AppointmentsMapper> _logger;
        private const string SessionTypeSeparator = " - ";

        public AppointmentsMapper(IDateTimeOffsetProvider dateTimeOffsetProvider, ILogger<AppointmentsMapper> logger)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
        }

        public IEnumerable<Appointment> Map(
            IEnumerable<Models.Appointment> sourceAppointments,
            IEnumerable<Location> locations,
            IEnumerable<SessionHolder> sessionHolders,
            IEnumerable<Models.Session> sessions)
        {
            var appointments = new List<Appointment>();

            if (sessions == null || !sessions.Any())
            {
                return appointments;
            }

            if (sourceAppointments == null || !sourceAppointments.Any())
            {
                return appointments;
            }

            var keyedSessions = sessions.ToDictionary(session => session.SessionId);

            foreach (var sourceAppointment in sourceAppointments)
            {
                var startTime = ParseSlotTime(sourceAppointment.StartTime, "Start");
                if (startTime == null)
                {
                    continue;
                }

                var endTime = ParseSlotTime(sourceAppointment.EndTime, "End");
                var sessionId = sourceAppointment.SessionId;

                var appointment = DateTime.Now >= startTime
                    ? (Appointment) new PastAppointment()
                    : new UpcomingAppointment();

                appointment.Id = sourceAppointment.SlotId.ToString(CultureInfo.InvariantCulture);
                appointment.StartTime = startTime.GetValueOrDefault();
                appointment.EndTime = endTime;
                appointment.Clinicians = FindCliniciansForSession(sessionId, keyedSessions, sessionHolders);
                appointment.Location = FindLocationForSession(sessionId, keyedSessions, locations);
                appointment.Type = CreateTypeFromAppointmentAndSession(sourceAppointment,
                    FindSession(sessionId, keyedSessions));

                appointments.Add(appointment);
            }

            return appointments;
        }

        private static string CreateTypeFromAppointmentAndSession(Models.Appointment appointment, Models.Session session)
        {
            var hasOnlyAppointmentSlotTypeOrSessionName = string.IsNullOrEmpty(appointment.SlotTypeName) || string.IsNullOrEmpty(session?.SessionName);
            return $"{session?.SessionName}{(hasOnlyAppointmentSlotTypeOrSessionName ? string.Empty : SessionTypeSeparator)}{appointment.SlotTypeName}";
        }

        private static IEnumerable<string> FindCliniciansForSession(
            int sessionId, 
            IReadOnlyDictionary<int, Models.Session> sessions,
            IEnumerable<SessionHolder> sessionHolders
            )
        {
            var session = FindSession(sessionId, sessions);
            var clinicianIds = session?.ClinicianIds ?? Array.Empty<int>();
            return sessionHolders?.Where(s => clinicianIds.Contains(s.ClinicianId))
                       .Select(s => s.DisplayName) ?? Array.Empty<string>();
        }

        private static string FindLocationForSession(
            int sessionId,
            IReadOnlyDictionary<int, Models.Session> sessions,
            IEnumerable<Location> locations)
        {
            var session = FindSession(sessionId, sessions);
            var location = locations?.FirstOrDefault(x => x.LocationId == session?.LocationId);
            return location?.LocationName ?? string.Empty;
        }

        private static Models.Session FindSession(int sessionId, IReadOnlyDictionary<int, Models.Session> sessions) =>
            sessions.ContainsKey(sessionId) ? sessions[sessionId] : null;

        private DateTimeOffset? ParseSlotTime(string time, string usage)
        {
            var success = _dateTimeOffsetProvider.TryCreateDateTimeOffset(time, out var parsedTime);
            if (!success)
            {
                _logger.LogError($"Unable to parse EMIS Appointment Slot {usage} Time of '{time}'");
            }
            return parsedTime;
        }
    }
}