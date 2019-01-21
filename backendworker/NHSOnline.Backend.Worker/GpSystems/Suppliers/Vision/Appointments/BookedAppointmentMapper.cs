using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public interface IBookedAppointmentMapper
    {
        IEnumerable<Appointment> Map(BookedAppointments bookedAppointments);
    }
    
    public class BookedAppointmentMapper: IBookedAppointmentMapper
    {
        private const string SessionTypeSeparator = " - ";
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public BookedAppointmentMapper(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public IEnumerable<Appointment> Map(BookedAppointments bookedAppointments)
        {
            var mappedAppointments = new List<Appointment>();
            
            if (bookedAppointments?.Slots == null)
                return mappedAppointments;
            
            var locations = SetUpLocations(bookedAppointments.References?.Locations);
            var slotTypes = SetUpSlotType(bookedAppointments.References?.SlotTypes);
            var sessions = SetUpSessions(bookedAppointments.References?.Sessions);
            var cancellationCutOffMinutes = bookedAppointments.Settings?.CancellationCutOffMinutes ?? 0;
            var currentTime = _dateTimeOffsetProvider.CreateDateTimeOffset();
            
            foreach (var slot in bookedAppointments.Slots)
            {

                if (!_dateTimeOffsetProvider.TryCreateDateTimeOffset(slot.DateTime, out var startTime))
                {
                    continue;
                }

                var location = locations.GetValueOrDefault(slot.Location, null);
                var slotType = slotTypes.GetValueOrDefault(slot.Type, null);
                var session = sessions.GetValueOrDefault(slot.Session, null);

                mappedAppointments.Add(new Appointment
                {
                    Id = slot.Id,
                    StartTime = startTime.GetValueOrDefault(),
                    EndTime = GetEndTime(startTime, slot),
                    Location = location,
                    Type = CreateTypeFromAppointmentAndSession(slotType, session),
                    Clinicians = GetClinician(slot.Owner, bookedAppointments.References?.Owners),
                    DisableCancellation = CutOffBreached(currentTime, startTime, cancellationCutOffMinutes)
                });
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

        private static DateTimeOffset? GetEndTime(DateTimeOffset? startTime, BookedSlot slot)
        {
            return startTime.HasValue && int.TryParse(slot.Duration, out var mins)
                ? (DateTimeOffset?)startTime.Value.AddMinutes(mins)
                : null;
        }

        private static bool CutOffBreached(DateTimeOffset currentTime, DateTimeOffset? startTime,
            int cancellationCutOffMinutes)
        {
            return startTime.HasValue && currentTime >= startTime.Value.AddMinutes(-cancellationCutOffMinutes);
        }
    }
}