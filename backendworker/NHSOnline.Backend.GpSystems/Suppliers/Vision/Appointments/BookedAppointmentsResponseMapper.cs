using System.Linq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments
{
    public interface IBookedAppointmentsResponseMapper
    {
        AppointmentsResponse Map(BookedAppointmentsResponse bookedAppointmentsResponse);
    }
    
    public class BookedAppointmentsResponseMapper : IBookedAppointmentsResponseMapper
    {
        private readonly IBookedAppointmentMapper _bookedAppointmentMapper;
        private readonly ICancellationReasonMapper _cancellationReasonMapper;

        public BookedAppointmentsResponseMapper(
            IBookedAppointmentMapper bookedAppointmentMapper, 
            ICancellationReasonMapper cancellationReasonMapper
            )
        {
            _bookedAppointmentMapper = bookedAppointmentMapper;
            _cancellationReasonMapper = cancellationReasonMapper;
        }
        
        public AppointmentsResponse Map(BookedAppointmentsResponse bookedAppointmentsResponse)
        {
            var appointments = _bookedAppointmentMapper.Map(bookedAppointmentsResponse.Appointments);
            var cancellationReasons = _cancellationReasonMapper.Map(bookedAppointmentsResponse);
            var response = new AppointmentsResponse
            {
                UpcomingAppointments = appointments, 
                CancellationReasons = cancellationReasons,
                DisableCancellation = !cancellationReasons.Any(),
                PastAppointmentsEnabled = false
            };

            return response;
        }
    }
}