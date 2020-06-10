using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments
{
    public interface IBookedAppointmentMapper
    {
        IEnumerable<UpcomingAppointment> Map(BookedAppointments bookedAppointments);
    }
    
    public class BookedAppointmentMapper: IBookedAppointmentMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<BookedAppointmentMapper> _logger;

        public BookedAppointmentMapper(
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            ILogger<BookedAppointmentMapper> logger)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
        }

        public IEnumerable<UpcomingAppointment> Map(BookedAppointments bookedAppointments)
        {
            var mappedAppointments = new List<UpcomingAppointment>();
            
            if (bookedAppointments?.Slots == null)
            {
                return mappedAppointments;
            }

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

                mappedAppointments.Add(new UpcomingAppointment
                {
                    Id = slot.Id,
                    StartTime = startTime.GetValueOrDefault(),
                    EndTime = GetEndTime(startTime, slot),
                    Location = location,
                    Clinicians = GetClinician(slot.Owner, bookedAppointments.References?.Owners),
                    DisableCancellation = CutOffBreached(currentTime, startTime, cancellationCutOffMinutes),
                    Type = string.IsNullOrWhiteSpace(slotType) 
                        ? string.Empty : slotType,
                    SessionName = string.IsNullOrWhiteSpace(session) 
                        ? string.Empty : session
                });
            }

            return mappedAppointments;
        }
        
        private Dictionary<string, string> SetUpLocations(IEnumerable<Location> locations)
        {
            return SetupDeduplicatedDictionary(locations, l => l.Id, l => l.Name);
        }
        
        private Dictionary<string, string> SetUpSlotType(IEnumerable<SlotType> slotTypes)
        {
            return SetupDeduplicatedDictionary(slotTypes, st => st.Id, st => st.Description);
        }
        
        private Dictionary<string, string> SetUpSessions(IEnumerable<SlotSession> sessions)
        {
            return SetupDeduplicatedDictionary(sessions, s => s.Id, s => s.Description);
        }

        private Dictionary<string, string> SetupDeduplicatedDictionary<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, string> keySelector,
            Func<TSource, string> valueSelector)
        {
            if (source == null)
            {
                return new Dictionary<string, string>();
            }

            return source
                .Select(s => new { Key = keySelector(s), Value = valueSelector(s) })
                .Distinct()
                .ToDictionaryLogOnFailure(x => x.Key, x => x.Value, _logger);
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
            return startTime.HasValue && int.TryParse(slot.Duration, out var minutes)
                ? (DateTimeOffset?)startTime.Value.AddMinutes(minutes)
                : null;
        }

        private static bool CutOffBreached(DateTimeOffset currentTime, DateTimeOffset? startTime,
            int cancellationCutOffMinutes)
        {
            return startTime.HasValue && currentTime >= startTime.Value.AddMinutes(-cancellationCutOffMinutes);
        }
    }
}