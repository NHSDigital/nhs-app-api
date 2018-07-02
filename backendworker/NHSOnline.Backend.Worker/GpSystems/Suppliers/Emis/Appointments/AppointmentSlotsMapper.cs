using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Date;
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
        private const string SessionTypeSeperator = " - ";

        public AppointmentSlotsMapper(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public IEnumerable<Slot> Map(
            IEnumerable<AppointmentSlotSession> slotSessions, 
            IEnumerable<Location> locations, 
            IEnumerable<SessionHolder> sessionHolders,
            IEnumerable<Models.Session> sessions)
        {
            var slots = new List<Slot>();

            if (sessions == null || !sessions.Any())
            {
                return slots;
            }
            
            if (slotSessions==null || ! slotSessions.Any())
            {
                return slots;
            }
            
            foreach (var sourceSlotSession in slotSessions)
            {
                foreach (var sourceSlot in sourceSlotSession.Slots)
                {
                    DateTimeOffset startTime;
                    
                    try
                    {
                        startTime = _dateTimeOffsetProvider.CreateDateTimeOffset(sourceSlot.StartTime).ToUniversalTime();
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    DateTimeOffset? endTime;
                    try
                    {
                        endTime = _dateTimeOffsetProvider.CreateDateTimeOffset(sourceSlot.EndTime).ToUniversalTime();
                    }
                    catch (Exception)
                    {
                        endTime = null;
                    }
                    
                    var sessionId = sourceSlotSession.SessionId;
                    var slot = new Slot
                    {
                        Id = sourceSlot.SlotId.ToString(),
                        StartTime = startTime,
                        EndTime = endTime,
                        Clinicians = FindCliniciansForSession(sessionId, sessions, sessionHolders),
                        Location = FindLocationForSession(sessionId, sessions, locations),
                        Type = CreateTypeFromSlotAndSession(sourceSlot,sessions.FirstOrDefault(x=>x.SessionId==sourceSlotSession.SessionId))
                    };

                    slots.Add(slot);
                }
            }

            return slots;
        }

        private string CreateTypeFromSlotAndSession(AppointmentSlot sourceSlot, Models.Session session)
        {
            var hasOnlySlotTypeOrSessionName = string.IsNullOrEmpty(sourceSlot.SlotTypeName) || string.IsNullOrEmpty(session?.SessionName);
            return $"{session?.SessionName}{(hasOnlySlotTypeOrSessionName ? string.Empty : SessionTypeSeperator)}{sourceSlot.SlotTypeName}";
        }

        private static string[] FindCliniciansForSession(int sessionId, IEnumerable<Models.Session> sessions, IEnumerable<SessionHolder> sessionHolders)
        {
            var session = sessions.FirstOrDefault(x => x.SessionId == sessionId);
            return session?.ClinicianIds == null ? new string[] { } : session.ClinicianIds.Select(x => sessionHolders?.FirstOrDefault(s=>s.ClinicianId==x).DisplayName).ToArray();
        }
        
        private static string FindLocationForSession(int sessionId, IEnumerable<Models.Session> sessions, IEnumerable<Models.Location> locations)
        {
            var session = sessions.FirstOrDefault(x => x.SessionId == sessionId);
            var location = locations.FirstOrDefault(x => x.LocationId == session?.LocationId);
            return location == null ? string.Empty : location.LocationName;
        }
    }
}
