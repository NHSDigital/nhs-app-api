using NHSOnline.Backend.Worker.Router.Appointment;
using NHSOnline.Backend.Worker.Router.Appointments;
using NHSOnline.Backend.Worker.Router.Im1Connection;
using NHSOnline.Backend.Worker.Router.Session;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Router
{
    public interface ISystemProvider
    {
        SupplierEnum Supplier { get; }

        IAppointmentsService GetAppointmentsService();
        IIm1ConnectionService GetIm1ConnectionService();
        ISessionService GetSessionService();
        ITokenValidationService GetTokenValidationService();
        IPrescriptionService GetPrescriptionService();
        IAppointmentSlotsService GetAppointmentService(UserSession userSession);
    }
}
