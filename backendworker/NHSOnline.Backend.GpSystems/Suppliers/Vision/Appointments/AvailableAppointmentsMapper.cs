using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Support.Temporal;
using Slot = NHSOnline.Backend.GpSystems.Appointments.Models.Slot;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments
{
    public interface IAvailableAppointmentsMapper
    {
        IEnumerable<Slot> Map(AvailableAppointments availableAppointments);
    }
    
    public class AvailableAppointmentsMapper: IAvailableAppointmentsMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<AvailableAppointmentsMapper> _logger;

        public AvailableAppointmentsMapper(IDateTimeOffsetProvider dateTimeOffsetProvider, ILogger<AvailableAppointmentsMapper> logger)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
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
                var slotType = slotTypes.GetValueOrDefault(slot.Type, null);
                if (string.IsNullOrWhiteSpace(slotType))
                {
                    _logger.LogWarning("Unable to parse Vision Appointment Slot - slot type name null or empty");
                    continue;
                }

                if (!_dateTimeOffsetProvider.TryCreateDateTimeOffset(slot.DateTime, out var startTime))
                {
                    _logger.LogWarning($"Unable to parse Vision appointment slot start time of '{slot.DateTime}'");
                    continue;
                }
                    
                var location = locations.GetValueOrDefault(slot.Location, null);
                var session = sessions.GetValueOrDefault(slot.Session, string.Empty);

                mappedAppointments.Add(
                    new Slot
                {
                    Id = slot.Id,
                    StartTime = startTime.GetValueOrDefault(),
                    EndTime = GetEndTime(startTime, slot),
                    Location = location,
                    Type = slotType,
                    SessionName = session,
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