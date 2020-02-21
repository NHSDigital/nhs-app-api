using System.Linq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments
{
    public interface IAppointmentsResponseMapper
    {
        AppointmentsResponse Map(AppointmentsGetResponse sourceAppointments);
    }

    public class AppointmentsResponseMapper : IAppointmentsResponseMapper
    {
        private readonly IAppointmentsMapper _appointmentsMapper;
        private readonly ICancellationReasonService _defaultCancellationReasons;

        public AppointmentsResponseMapper(
            IAppointmentsMapper appointmentsMapper,
            ICancellationReasonService defaultCancellationReasons)
        {
            _appointmentsMapper = appointmentsMapper;
            _defaultCancellationReasons = defaultCancellationReasons;
        }

        public AppointmentsResponse Map(AppointmentsGetResponse sourceAppointments)
        {
            var allAppointments = _appointmentsMapper.Map(
                sourceAppointments.Appointments,
                sourceAppointments.Locations,
                sourceAppointments.SessionHolders,
                sourceAppointments.Sessions);

            var pastAppointments = allAppointments.Where(x => x is PastAppointment).Cast<PastAppointment>();
            var upcomingAppointments = allAppointments
                .Where(x => x is UpcomingAppointment).Cast<UpcomingAppointment>();

            var cancellationReasons = _defaultCancellationReasons.GetDefaultCancellationReasons();

            var response = new AppointmentsResponse
            {
                PastAppointments = pastAppointments,
                UpcomingAppointments = upcomingAppointments,
                CancellationReasons = cancellationReasons,
                PastAppointmentsEnabled = true
            };

            return response;
        }
    }
}