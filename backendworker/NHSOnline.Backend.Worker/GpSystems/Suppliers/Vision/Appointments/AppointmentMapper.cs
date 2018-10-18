using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public interface IAppointmentMapper
    {
        IEnumerable<Appointment> Map(BookedAppointmentsResponse appointmentsResponses);
    }
    
    public class AppointmentMapper : IAppointmentMapper
    {
        private const string SessionTypeSeparator = " - ";
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public AppointmentMapper(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }
        
        public IEnumerable<Appointment> Map(BookedAppointmentsResponse appointmentsResponses)
        {
            var appointments = new List<Appointment>();
            
            if (appointmentsResponses?.Appointments?.Slots == null)
                return appointments;
            
            var now = _dateTimeOffsetProvider.CreateDateTimeOffset();
            var locations = SetUpLocations(appointmentsResponses.Appointments?.References?.Locations);
            var slotTypes = SetUpSlotType(appointmentsResponses.Appointments?.References?.SlotTypes);
            var sessions = SetUpSessions(appointmentsResponses.Appointments?.References?.Sessions);
            
            foreach (var slot in appointmentsResponses.Appointments.Slots)
            {
                if (!_dateTimeOffsetProvider.TryCreateDateTimeOffset(slot.DateTime, out var startTime))
                {
                    continue;
                }

                var location = locations.GetValueOrDefault(slot.Location, null);
                var slotType = slotTypes.GetValueOrDefault(slot.Type, null);
                var session = sessions.GetValueOrDefault(slot.Session, null);
                
                var appointment = new Appointment
                {
                    Id = slot.Id,
                    StartTime = startTime.GetValueOrDefault(),
                    EndTime = GetEndTime(startTime, slot),
                    Location = location,
                    Type = CreateTypeFromAppointmentAndSession(slotType, session),
                    Clinicians = GetClinician(slot.Owner, appointmentsResponses.Appointments?.References?.Owners)
                };
                
                appointments.Add(appointment);
            }

            return appointments;
        }

        private static Dictionary<string, string> SetUpLocations(IEnumerable<Location> locations)
        {
            return locations == null
                ? new Dictionary<string, string>()
                : locations.ToDictionary(location => location.Id, location => location.Name);
        }
        
        private static Dictionary<string, string> SetUpSlotType(IEnumerable<SlotType> slotTypes)
        {
            return slotTypes == null
                ? new Dictionary<string, string>()
                : slotTypes.ToDictionary(slotType => slotType.Id, slotType => slotType.Description);
        }
        
        private static Dictionary<string, string> SetUpSessions(IEnumerable<SlotSession> sessions)
        {
            return sessions == null
                ? new Dictionary<string, string>()
                : sessions.ToDictionary(session => session.Id, session => session.Description);
        }
        
        private static string CreateTypeFromAppointmentAndSession(string slotType, string session)
        {
            var hasOnlyAppointmentSlotTypeOrSessionName =
                string.IsNullOrEmpty(slotType) || string.IsNullOrEmpty(session);
            return
                $"{session}{(hasOnlyAppointmentSlotTypeOrSessionName ? string.Empty : SessionTypeSeparator)}{slotType}";
        }

        private static IEnumerable<string> GetClinician(string ownerId, IEnumerable<Owner> owners)
        {
            return string.IsNullOrEmpty(ownerId)
                ? Array.Empty<string>()
                : owners?.Where(owner => owner.Id.Equals(ownerId, StringComparison.Ordinal))
                      .Select(owner => owner.Name) ?? Array.Empty<string>();
        }

        private static DateTimeOffset? GetEndTime(DateTimeOffset? startTime, BookedSlot slot)
        {
            return startTime.HasValue && int.TryParse(slot.Duration, out var mins)
                ? (DateTimeOffset?)startTime.Value.AddMinutes(mins)
                : null;
        }
    }
}