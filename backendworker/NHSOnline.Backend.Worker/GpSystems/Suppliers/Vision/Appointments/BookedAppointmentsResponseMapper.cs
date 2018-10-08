using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public interface IBookedAppointmentsResponseMapper
    {
        AppointmentsResponse Map(BookedAppointmentsResponse bookedAppointmentsResponse);
    }
    
    public class BookedAppointmentsResponseMapper : IBookedAppointmentsResponseMapper
    {
        private readonly IAppointmentMapper _appointmentMapper;
        private readonly ICancellationReasonMapper _cancellationReasonMapper;

        public BookedAppointmentsResponseMapper(
            IAppointmentMapper appointmentMapper, 
            ICancellationReasonMapper cancellationReasonMapper
            )
        {
            _appointmentMapper = appointmentMapper;
            _cancellationReasonMapper = cancellationReasonMapper;
        }
        
        public AppointmentsResponse Map(BookedAppointmentsResponse bookedAppointmentsResponse)
        {
            var appointments = _appointmentMapper.Map(bookedAppointmentsResponse);
            var cancellationReasons = _cancellationReasonMapper.Map(bookedAppointmentsResponse);
            var response = new AppointmentsResponse
            {
                Appointments = appointments, 
                CancellationReasons = cancellationReasons
            };

            return response;
        }
    }
}