using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using Slot = NHSOnline.Backend.Worker.Areas.Appointments.Models.Slot;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public interface IAvailableAppointmentsResponseMapper
    {
        AppointmentSlotsResponse Map(AvailableAppointmentsResponse availableAppointmentsResponse);
    }
    
    public class AvailableAppointmentsResponseMapper : IAvailableAppointmentsResponseMapper
    {
        private readonly IAvailableAppointmentsMapper _availableAppointmentMapper;
        
        public AvailableAppointmentsResponseMapper(IAvailableAppointmentsMapper availableAppointmentMapper)
        {
            _availableAppointmentMapper = availableAppointmentMapper;
        }
        
        public AppointmentSlotsResponse Map(AvailableAppointmentsResponse availableAppointmentsResponse)
        {
            var slots =_availableAppointmentMapper.Map(availableAppointmentsResponse.Appointments);

            return new AppointmentSlotsResponse { Slots = slots};
        }
    }
}