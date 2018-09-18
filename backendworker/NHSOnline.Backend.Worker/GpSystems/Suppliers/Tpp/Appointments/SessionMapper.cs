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
        IEnumerable<Areas.Appointments.Models.Slot> Map(IEnumerable<Models.Appointments.Session> sessions);
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
        
        public IEnumerable<Areas.Appointments.Models.Slot> Map(IEnumerable<Models.Appointments.Session> sessions)
        {
            if (sessions == null)
                yield break;

            foreach(var session in sessions.Where(s => s.Slots!=null))
            {
                foreach(var slot in session.Slots)
                {
                    DateTimeOffset startDate;

                    try
                    {
                        startDate = _dateTimeOffsetProvider.CreateDateTimeOffset(slot.StartDate.TrimEnd('Z'));
                    }
                    catch
                    {
                        continue;
                    }

                    DateTimeOffset? endDate = null;
                    if (!string.IsNullOrEmpty(slot.EndDate))
                    {
                        try
                        {
                            endDate = _dateTimeOffsetProvider.CreateDateTimeOffset(slot.EndDate.TrimEnd('Z'));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Unable to create DateTimeOffset from slot EndDate '{0}'.",
                                slot.EndDate);
                            endDate = null;
                        }
                    }

                    yield return new Areas.Appointments.Models.Slot
                    {
                        Clinicians = GetCliniciansForSession(session),
                        EndTime = endDate,
                        Id = session.SessionId,
                        Location = session.Location,
                        StartTime = startDate,
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
