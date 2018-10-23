using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.Support.Temporal;
using Slot = NHSOnline.Backend.Worker.Areas.Appointments.Models.Slot;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public interface IAvailableAppointmentsMapper
    {
        IEnumerable<Slot> Map(AvailableAppointments availableAppointments);
    }
    
    public class AvailableAppointmentsMapper: IAvailableAppointmentsMapper
    {
        private const string SessionTypeSeparator = " - ";
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public AvailableAppointmentsMapper(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }
        
        public IEnumerable<Slot> Map(AvailableAppointments availableAppointments)
        {
            var mappedAppointments = new List<Slot>();
            
            if (availableAppointments?.Slots == null)
                return mappedAppointments;
            
            var locations = SetUpLocations(availableAppointments.References?.Locations);
            var slotTypes = SetUpSlotType(availableAppointments.References?.SlotTypes);
            var sessions = SetUpSessions(availableAppointments.References?.Sessions);
            
            foreach (var slot in availableAppointments.Slots)
            {

                if (!_dateTimeOffsetProvider.TryCreateDateTimeOffset(slot.DateTime, out var startTime))
                    continue;

                var location = locations.GetValueOrDefault(slot.Location, null);
                var slotType = slotTypes.GetValueOrDefault(slot.Type, null);
                var session = sessions.GetValueOrDefault(slot.Session, null);

                mappedAppointments.Add(
                    new Slot
                {
                    Id = slot.Id,
                    StartTime = startTime.GetValueOrDefault(),
                    EndTime = GetEndTime(startTime, slot),
                    Location = location,
                    Type = CreateTypeFromAppointmentAndSession(slotType, session),
                    Clinicians = GetClinician(slot.Owner, availableAppointments.References?.Owners)
                }
                    );
            }

            return mappedAppointments;
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
        
        private static DateTimeOffset? GetEndTime(DateTimeOffset? startTime, FreeSlot slot)
        {
            return startTime.HasValue && int.TryParse(slot.Duration, out var mins)
                ? (DateTimeOffset?)startTime.Value.AddMinutes(mins)
                : null;
        }
    }
}