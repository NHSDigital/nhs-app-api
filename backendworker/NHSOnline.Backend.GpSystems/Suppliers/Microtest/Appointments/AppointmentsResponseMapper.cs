using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Temporal;
using Appointment = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments.Appointment;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class AppointmentsResponseMapper : IAppointmentsResponseMapper
    {
        private readonly ILogger<AppointmentsResponseMapper> _logger;
        private readonly ICancellationReasonService _defaultCancellationReasons;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public AppointmentsResponseMapper(
            ILogger<AppointmentsResponseMapper> logger,
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            ICancellationReasonService defaultCancellationReasons)
        {
            _logger = logger;
            _defaultCancellationReasons = defaultCancellationReasons;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public AppointmentsResponse Map(AppointmentsGetResponse appointmentsGetResponse)
        {
            _logger.LogEnter();

            var upcomingAppointments = new List<UpcomingAppointment>();
            var pastAppointments = new List<PastAppointment>();

            var now = _dateTimeOffsetProvider.CreateDateTimeOffset();

            foreach (var sourceAppointment in appointmentsGetResponse.Appointments)
            {
                if (sourceAppointment.StartTime == null)
                {
                    continue;
                }

                if (sourceAppointment.StartTime < now)
                {
                    var resultAppointment = new PastAppointment();
                    MapAppointment(sourceAppointment, resultAppointment);
                    pastAppointments.Add(resultAppointment);
                }
                else
                {
                    var resultAppointment = new UpcomingAppointment();
                    MapAppointment(sourceAppointment, resultAppointment);
                    upcomingAppointments.Add(resultAppointment);
                }
            }

            var cancellationReasons = _defaultCancellationReasons.GetDefaultCancellationReasons();

            _logger.LogExit();

            return new AppointmentsResponse
            {
                UpcomingAppointments = upcomingAppointments,
                PastAppointments = pastAppointments,
                PastAppointmentsEnabled = true,
                CancellationReasons = cancellationReasons
            };
        }

        private static void MapAppointment(Appointment sourceAppointment, GpSystems.Appointments.Models.Appointment resultAppointment)
        {
            resultAppointment.Id = sourceAppointment.Id;
            resultAppointment.StartTime = sourceAppointment.StartTime.Value;
            resultAppointment.EndTime = sourceAppointment.EndTime;
            resultAppointment.Clinicians = sourceAppointment.Clinicians;
            resultAppointment.Location = sourceAppointment.Location;
            resultAppointment.Type = sourceAppointment.Type;
            resultAppointment.SessionName = string.Empty;
        }
    }
}