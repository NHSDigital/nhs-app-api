using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public class AppointmentSessionMapper : BaseMapper<Models.Session, AppointmentSession>
    {
        public override AppointmentSession Map(Models.Session source)
        {
            return new AppointmentSession
            {
                Id = source.SessionId.ToString(),
                DisplayName = source.SessionType
            };
        }
    }
}
