using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public interface IAppointmentSlotsMapper
    {
        IList<Slot> Map(
            IEnumerable<Suppliers.Microtest.Models.Appointments.Slot> sourceSlots);
    }
}