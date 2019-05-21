using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public interface IAppointmentSlotsMapper
    {
        AppointmentSlotsResponse Map(
            ListSlotsReply listSlotsReply, 
            RequestSystmOnlineMessagesReply messagesReply);
    }
}
