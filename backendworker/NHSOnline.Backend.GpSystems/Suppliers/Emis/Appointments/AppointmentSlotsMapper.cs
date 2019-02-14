using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support.Temporal;
using Location = NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments
{
    public interface IAppointmentSlotsMapper
    {
        IEnumerable<Slot> Map(IEnumerable<AppointmentSlotSession> slotSessions, IEnumerable<Location> locations, IEnumerable<SessionHolder> sessionHolders, IEnumerable<Models.Session> sessions);
    }

    public class AppointmentSlotsMapper : IAppointmentSlotsMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<AppointmentSlotsMapper> _logger;
        private const string SessionTypeSeparator = " - ";
        private readonly IEmisEnumMapper _emisEnumMapper;
        
        public AppointmentSlotsMapper(IDateTimeOffsetProvider dateTimeOffsetProvider, ILogger<AppointmentSlotsMapper> logger, IEmisEnumMapper emisEnumMapper)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
            _emisEnumMapper = emisEnumMapper;
        }

        public IEnumerable<Slot> Map(
            IEnumerable<AppointmentSlotSession> slotSessions, 
            IEnumerable<Location> locations, 
            IEnumerable<SessionHolder> sessionHolders,
            IEnumerable<Models.Session> sessions)
        {
            if (slotSessions == null || !slotSessions.Any())
            {
                yield break;
            }

            if (sessions == null || !sessions.Any())
            {
                yield break;
            }

            var keyedSessions = sessions.ToDictionary(session => session.SessionId);
            
            foreach (var sourceSlotSession in slotSessions)
            {
                foreach (var sourceSlot in sourceSlotSession.Slots)
                {
                    var startTime = ParseSlotTime(sourceSlot.StartTime, "Start");
                    if (startTime == null)
                    {
                        continue;
                    }
                    var endTime = ParseSlotTime(sourceSlot.EndTime, "End");
                    
                    var sessionId = sourceSlotSession.SessionId;
                    
                    var slot = new Slot
                    {
                        Id = sourceSlot.SlotId.ToString(CultureInfo.InvariantCulture),
                        StartTime = startTime.GetValueOrDefault(),
                        EndTime = endTime,
                        Clinicians = FindCliniciansForSession(sessionId, keyedSessions, sessionHolders),
                        Location = FindLocationForSession(sessionId, keyedSessions, locations),
                        Type = CreateTypeFromSlotAndSession(sourceSlot, FindSession(sessionId, keyedSessions)), 
                        Channel = _emisEnumMapper.MapSlotTypeStatus(sourceSlot.SlotTypeStatus, Channel.Unknown)             
                    };

                    yield return slot;
                }
            }
        }

        private static string CreateTypeFromSlotAndSession(AppointmentSlot sourceSlot, Models.Session session)
        {
            var hasOnlySlotTypeOrSessionName = string.IsNullOrEmpty(sourceSlot.SlotTypeName) || string.IsNullOrEmpty(session?.SessionName);
            return $"{session?.SessionName}{(hasOnlySlotTypeOrSessionName ? string.Empty : SessionTypeSeparator)}{sourceSlot.SlotTypeName}";
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

        private static string FindLocationForSession(int sessionId, IReadOnlyDictionary<int, Models.Session> sessions, IEnumerable<Location> locations)
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