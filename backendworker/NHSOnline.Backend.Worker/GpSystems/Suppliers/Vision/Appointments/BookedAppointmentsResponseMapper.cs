using System.Linq;
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
                Appointments = appointments, 
                CancellationReasons = cancellationReasons,
                DisableCancellation = !cancellationReasons.Any()
            };

            return response;
        }
    }
}