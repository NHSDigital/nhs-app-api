using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public class SessionMapper : ISessionMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<SessionMapper> _logger;

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
                foreach (var sourceSlot in session.Slots)
                {
                    if (string.IsNullOrWhiteSpace(sourceSlot.Type))
                    {
                        _logger.LogWarning("Unable to parse TPP Appointment Slot - slot type name null or empty");
                        continue;
                    }
                    var startDateSuccess = _dateTimeOffsetProvider.TryCreateDateTimeOffset(sourceSlot.StartDate?.TrimEnd('Z'),
                        out var startDate);
                    if (!startDateSuccess)
                    {
                        _logger.LogWarning($"Unable to parse TPP Appointment Slot - wrong start date: {sourceSlot.StartDate}");
                        continue;
                    }

                    var endDateSuccess = _dateTimeOffsetProvider.TryCreateDateTimeOffset(sourceSlot.EndDate?.TrimEnd('Z'),
                        out var endDate);

                    if (!endDateSuccess)
                    {
                        _logger.LogWarning($"Unable to create {nameof(DateTimeOffset)} from slot {nameof(sourceSlot.EndDate)} '{sourceSlot.EndDate}'.");
                    }

                    yield return new GpSystems.Appointments.Models.Slot
                    {
                        Clinicians = GetCliniciansForSession(session),
                        EndTime = endDate,
                        Id = session.SessionId,
                        Location = session.Location,
                        StartTime = startDate.GetValueOrDefault(),
                        Type = sourceSlot.Type.Trim(),
                        SessionName = 
                            string.IsNullOrWhiteSpace(session.Type) ? string.Empty : session.Type.Trim(),
                    };
                }
            }
        }

        private static string[] GetCliniciansForSession(Models.Appointments.Session session)
        {
            return string.IsNullOrEmpty(session.StaffDetails) ? Array.Empty<string>() : new[] { session.StaffDetails };
        }
    }
}
