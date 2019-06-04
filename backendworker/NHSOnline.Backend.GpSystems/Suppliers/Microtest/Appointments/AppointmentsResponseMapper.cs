using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class AppointmentsResponseMapper : IAppointmentsResponseMapper
    {
        private readonly ILogger<AppointmentsResponseMapper> _logger;
        private readonly ICancellationReasonService _defaultCancellationReasons;
        private readonly IAppointmentsMapper _appointmentsMapper;
        
        public AppointmentsResponseMapper(
            ILogger<AppointmentsResponseMapper> logger,
            ICancellationReasonService defaultCancellationReasons,
            IAppointmentsMapper appointmentsMapper)
        {
            _logger = logger;
            _defaultCancellationReasons = defaultCancellationReasons;
            _appointmentsMapper = appointmentsMapper;
        }

        public AppointmentsResponse Map(AppointmentsGetResponse appointmentsGetResponse)
        {
            _logger.LogEnter();

            var allAppointments = _appointmentsMapper.Map(appointmentsGetResponse.Appointments);
            
            var pastAppointments = allAppointments.Where(x => x is PastAppointment).Cast<PastAppointment>();
            var upcomingAppointments = allAppointments
                .Where(x => x is UpcomingAppointment).Cast<UpcomingAppointment>();

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
    }
}