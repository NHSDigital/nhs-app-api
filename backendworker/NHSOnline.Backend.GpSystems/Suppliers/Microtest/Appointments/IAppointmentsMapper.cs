using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public interface IAppointmentsMapper
    {
        IEnumerable<Appointment> Map(
            IEnumerable<Suppliers.Microtest.Models.Appointments.Appointment> sourceAppointments);
    }
}