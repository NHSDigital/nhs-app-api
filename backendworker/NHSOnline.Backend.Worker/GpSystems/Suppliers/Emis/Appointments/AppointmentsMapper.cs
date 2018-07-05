using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Date;
using Appointment = NHSOnline.Backend.Worker.Areas.Appointments.Models.Appointment;
using Location = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public interface IAppointmentsMapper
    {
        IEnumerable<Appointment> Map(IEnumerable<Models.Appointment> appointmentsResponseAppointments, IEnumerable<Location> appointmentsResponseLocations, IEnumerable<SessionHolder> appointmentsResponseSessionHolders, IEnumerable<Models.Session> appointmentsResponseSessions);
    }

    public class AppointmentsMapper : IAppointmentsMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private const string SessionTypeSeparator = " - ";

        public AppointmentsMapper(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public IEnumerable<Appointment> Map(
            IEnumerable<Models.Appointment> sourceAppointments, 
            IEnumerable<Location> locations,
            IEnumerable<SessionHolder> sessionHolders, 
            IEnumerable<Models.Session> sessions)
        {
            var appointments = new List<Appointment>();

            if (sessions == null || !sessions.Any())
            {
                return appointments;
            }

            if (sourceAppointments == null || !sourceAppointments.Any())
            {
                return appointments;
            }

            foreach (var sourceAppointment in sourceAppointments)
            {
                DateTimeOffset startTime;

                try
                {
                    startTime = _dateTimeOffsetProvider.CreateDateTimeOffset(sourceAppointment.StartTime);
                }
                catch (Exception)
                {
                    continue;
                }

                DateTimeOffset? endTime;
                try
                {
                    endTime = _dateTimeOffsetProvider.CreateDateTimeOffset(sourceAppointment.EndTime);
                }
                catch (Exception)
                {
                    endTime = null;
                }

                var sessionId = sourceAppointment.SessionId;

                var appointment = new Areas.Appointments.Models.Appointment
                {
                    Id = sourceAppointment.SlotId.ToString(),
                    StartTime = startTime,
                    EndTime = endTime,
                    Clinicians = FindCliniciansForSession(sessionId, sessions, sessionHolders),
                    Location = FindLocationForSession(sessionId, sessions, locations),
                    Type = CreateTypeFromAppointmentAndSession(sourceAppointment, sessions.FirstOrDefault(x=>x.SessionId==sourceAppointment.SessionId))
                };

                appointments.Add(appointment);
            }

            return appointments;
        }

        private string CreateTypeFromAppointmentAndSession(Models.Appointment appointment, Models.Session session)
        {
            var hasOnlyAppointmentSlotTypeOrSessionName = string.IsNullOrEmpty(appointment.SlotTypeName) || string.IsNullOrEmpty(session?.SessionName);
            return $"{session?.SessionName}{(hasOnlyAppointmentSlotTypeOrSessionName ? string.Empty : SessionTypeSeparator)}{appointment.SlotTypeName}";
        }

        private static string[] FindCliniciansForSession(int sessionId, IEnumerable<Models.Session> sessions, IEnumerable<SessionHolder> sessionHolders)
        {
            var session = sessions.FirstOrDefault(x => x.SessionId == sessionId);
            return session?.ClinicianIds == null ? new string[] { } : session.ClinicianIds.Select(x => sessionHolders?.FirstOrDefault(s => s.ClinicianId == x).DisplayName).ToArray();
        }

        private static string FindLocationForSession(int sessionId, IEnumerable<Models.Session> sessions, IEnumerable<Models.Location> locations)
        {
            var session = sessions.FirstOrDefault(x => x.SessionId == sessionId);
            var location = locations.FirstOrDefault(x => x.LocationId == session?.LocationId);
            return location == null ? string.Empty : location.LocationName;
        }
    }
}