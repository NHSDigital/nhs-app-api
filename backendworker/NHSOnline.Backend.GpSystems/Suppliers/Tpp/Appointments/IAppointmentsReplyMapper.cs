using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public interface IAppointmentsReplyMapper
    {
        AppointmentsResponse Map(ViewAppointmentsReply viewPastAppointmentsReply, ViewAppointmentsReply viewUpcomingAppointmentsReply);
    }
}