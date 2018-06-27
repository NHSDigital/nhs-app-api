using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public class AppointmentClinicianMapper : BaseMapper<SessionHolder, Clinician>
    {
        public override Clinician Map(SessionHolder source)
        {
            return new Clinician
            {
                Id = source.ClinicianId.ToString(),
                DisplayName = source.DisplayName
            };
        }
    }
}
