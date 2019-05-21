using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public interface IAppointmentMapper
    {
        IEnumerable<Appointment> Map(List<Models.Appointments.Appointment> sourceAppointments);
    }
}