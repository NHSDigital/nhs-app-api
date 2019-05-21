using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public class AppointmentSlotsMapper : IAppointmentSlotsMapper
    {
        private readonly ISessionMapper _sessionMapper;

        public AppointmentSlotsMapper(ISessionMapper sessionMapper)
        {
            _sessionMapper = sessionMapper;
        }
        
        public AppointmentSlotsResponse Map(
            ListSlotsReply listSlotsReply, 
            RequestSystmOnlineMessagesReply messagesReply)
        {
            var slots = _sessionMapper.Map(listSlotsReply?.Sessions);
            var guidance = messagesReply?.BookAppointments?.Trim() ?? string.Empty;

            var response = new AppointmentSlotsResponse
            {
                Slots = slots,
                BookingGuidance = guidance
            };

            return response;
        }
    }
}