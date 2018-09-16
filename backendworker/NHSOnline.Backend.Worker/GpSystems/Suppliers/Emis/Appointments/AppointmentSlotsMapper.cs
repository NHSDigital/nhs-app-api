using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Temporal;
using Location = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
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

        public AppointmentSlotsMapper(IDateTimeOffsetProvider dateTimeOffsetProvider, ILogger<AppointmentSlotsMapper> logger)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
        }

        public IEnumerable<Slot> Map(
            IEnumerable<AppointmentSlotSession> slotSessions, 
            IEnumerable<Location> locations, 
            IEnumerable<SessionHolder> sessionHolders,
            IEnumerable<Models.Session> sessions)
        {
            if (sessions == null || !sessions.Any())
            {
                yield break;
            }
            
            if (slotSessions==null || ! slotSessions.Any())
            {
                yield break;
            }
            
            foreach (var sourceSlotSession in slotSessions)
            {
                foreach (var sourceSlot in sourceSlotSession.Slots)
                {
                    DateTimeOffset startTime;
                    
                    try
                    {
                        startTime = _dateTimeOffsetProvider.CreateDateTimeOffset(sourceSlot.StartTime);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unable to parse EMIS Appointment Slot Start Time of '{}'", sourceSlot.StartTime);
                        continue;
                    }

                    DateTimeOffset? endTime;
                    try
                    {
                        endTime = _dateTimeOffsetProvider.CreateDateTimeOffset(sourceSlot.EndTime);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unable to parse EMIS Appointment Slot End Time of '{}'", sourceSlot.StartTime);
                        endTime = null;
                    }
                    
                    var sessionId = sourceSlotSession.SessionId;
                    var slot = new Slot
                    {
                        Id = sourceSlot.SlotId.ToString(CultureInfo.InvariantCulture),
                        StartTime = startTime,
                        EndTime = endTime,
                        Clinicians = FindCliniciansForSession(sessionId, sessions, sessionHolders),
                        Location = FindLocationForSession(sessionId, sessions, locations),
                        Type = CreateTypeFromSlotAndSession(sourceSlot,sessions.FirstOrDefault(x=>x.SessionId==sourceSlotSession.SessionId))
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

        private static string[] FindCliniciansForSession(int sessionId, IEnumerable<Models.Session> sessions, IEnumerable<SessionHolder> sessionHolders)
        {
            var session = sessions.FirstOrDefault(x => x.SessionId == sessionId);
            return session?.ClinicianIds == null ? Array.Empty<string>() : session.ClinicianIds.Select(x => sessionHolders?.FirstOrDefault(s=>s.ClinicianId==x).DisplayName).ToArray();
        }
        
        private static string FindLocationForSession(int sessionId, IEnumerable<Models.Session> sessions, IEnumerable<Location> locations)
        {
            var session = sessions.FirstOrDefault(x => x.SessionId == sessionId);
            var location = locations.FirstOrDefault(x => x.LocationId == session?.LocationId);
            return location == null ? string.Empty : location.LocationName;
        }
    }
}
