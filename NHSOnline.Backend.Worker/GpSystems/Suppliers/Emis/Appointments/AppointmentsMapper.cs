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
                    startTime = _dateTimeOffsetProvider.CreateDateTimeOffset(sourceAppointment.StartTime).ToUniversalTime();
                }
                catch (Exception)
                {
                    continue;
                }

                DateTimeOffset? endTime;
                try
                {
                    endTime = _dateTimeOffsetProvider.CreateDateTimeOffset(sourceAppointment.EndTime).ToUniversalTime();
                }
                catch (Exception)
                {
                    endTime = null;
                }

                var sessionId = sourceAppointment.SessionId;

                var appointment = new Areas.Appointments.Models.Appointment
                {
                    Id = sourceAppointment.SlotId.ToString(),
                    AppointmentSessionId = sessionId.ToString(),
                    StartTime = startTime,
                    EndTime = endTime,
                    ClinicianIds = FindClinicianIdsForSession(sessionId, sessions),
                    LocationId = FindLocationIdForSession(sessionId, sessions),
                    SlotType = sourceAppointment.SlotTypeName,
                };

                appointments.Add(appointment);
            }

            return appointments;
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