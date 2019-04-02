using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public interface IAppointmentsResponseMapper
    {
        AppointmentsResponse Map(AppointmentsGetResponse appointmentsGetResponse);
    }
}