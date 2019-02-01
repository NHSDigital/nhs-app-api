using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public interface ISessionMapper
    {
        IEnumerable<GpSystems.Appointments.Models.Slot> Map(IEnumerable<Models.Appointments.Session> sessions);
    }
    
    public class SessionMapper : ISessionMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<SessionMapper> _logger;
        private const string SessionTypeSeparator = " - ";

        public SessionMapper(IDateTimeOffsetProvider dateTimeOffsetProvider, ILogger<SessionMapper> logger)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
        }

        public IEnumerable<GpSystems.Appointments.Models.Slot> Map(IEnumerable<Models.Appointments.Session> sessions)
        {
            if (sessions == null)
                yield break;

            foreach (var session in sessions.Where(s => s.Slots != null))
            {
                foreach (var slot in session.Slots)
                {
                    var startDateSuccess = _dateTimeOffsetProvider.TryCreateDateTimeOffset(slot.StartDate?.TrimEnd('Z'),
                        out var startDate);
                    if (!startDateSuccess)
                    {
                        continue;
                    }

                    var endDateSuccess = _dateTimeOffsetProvider.TryCreateDateTimeOffset(slot.EndDate?.TrimEnd('Z'),
                        out var endDate);

                    if (!endDateSuccess)
                    {
                        _logger.LogWarning($"Unable to create {nameof(DateTimeOffset)} from slot {nameof(slot.EndDate)} '{slot.EndDate}'.");
                    }

                    yield return new GpSystems.Appointments.Models.Slot
                    {
                        Clinicians = GetCliniciansForSession(session),
                        EndTime = endDate,
                        Id = session.SessionId,
                        Location = session.Location,
                        StartTime = startDate.GetValueOrDefault(),
                        Type = CreateTypeFromSlotAndSession(slot, session)
                    };
                }
            }
        }

        private static string[] GetCliniciansForSession(Models.Appointments.Session session)
        {
            return string.IsNullOrEmpty(session.StaffDetails) ? Array.Empty<string>() : new[] { session.StaffDetails };
        }

        private static string CreateTypeFromSlotAndSession(Slot slot, Models.Appointments.Session session)
        {
            var hasOnlySlotTypeOrSessionName = string.IsNullOrEmpty(slot.Type) || string.IsNullOrEmpty(session?.Type);
            return $"{session?.Type}{(hasOnlySlotTypeOrSessionName ? string.Empty : SessionTypeSeparator)}{slot.Type}";
        }
    }
}
