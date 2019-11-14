using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public interface ISessionMapper
    {
        IList<GpSystems.Appointments.Models.Slot> Map(IEnumerable<Models.Appointments.Session> sessions);
    }
}