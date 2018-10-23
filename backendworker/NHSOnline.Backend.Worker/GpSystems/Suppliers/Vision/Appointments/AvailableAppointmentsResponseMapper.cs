using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public interface IAvailableAppointmentsResponseMapper
    {
        AppointmentSlotsResponse Map(AvailableAppointmentsResponse availableAppointmentsResponse,
            VisionUserSession userSession);
    }
    
    public class AvailableAppointmentsResponseMapper : IAvailableAppointmentsResponseMapper
    {
        private readonly IAvailableAppointmentsMapper _availableAppointmentMapper;
        
        public AvailableAppointmentsResponseMapper(IAvailableAppointmentsMapper availableAppointmentMapper)
        {
            _availableAppointmentMapper = availableAppointmentMapper;
        }
        
        public AppointmentSlotsResponse Map(AvailableAppointmentsResponse availableAppointmentsResponse,
            VisionUserSession userSession)
        {
            var slots =_availableAppointmentMapper.Map(availableAppointmentsResponse.Appointments);

            return new AppointmentSlotsResponse
            {
                Slots = slots,
                BookingReasonNecessity = userSession.AppointmentBookingReasonNecessity
            };
        }
    }
}