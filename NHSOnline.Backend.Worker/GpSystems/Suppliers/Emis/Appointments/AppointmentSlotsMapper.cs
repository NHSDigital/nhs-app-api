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
                        AppointmentSessionId = sessionId.ToString(),
                        StartTime = startTime,
                        EndTime = endTime,
                        ClinicianIds = FindClinicianIdsForSession(sessionId, sessions),
                        LocationId = FindLocationIdForSession(sessionId, sessions)
                    };

                    slots.Add(slot);
                }
            }

            return slots;
        }

        private static string[] FindClinicianIdsForSession(int sessionId, IEnumerable<Models.Session> sessions)
        {
            var session = sessions.FirstOrDefault(x => x.SessionId == sessionId);

            return session == null ? new string[] { } : session.ClinicianIds.Select(x => x.ToString()).ToArray();
        }
        
        private static string FindLocationIdForSession(int sessionId, IEnumerable<Models.Session> sessions)
        {
            var session = sessions.FirstOrDefault(x => x.SessionId == sessionId);

            return session == null ? string.Empty : session.LocationId.ToString();
        }
    }
}
