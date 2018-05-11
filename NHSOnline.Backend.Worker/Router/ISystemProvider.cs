using NHSOnline.Backend.Worker.Router.Appointments;
using NHSOnline.Backend.Worker.Router.Im1Connection;
using NHSOnline.Backend.Worker.Router.Session;

namespace NHSOnline.Backend.Worker.Router
{
    public interface ISystemProvider
    {
        SupplierEnum Supplier { get; }

        IAppointmentsService GetAppointmentsService();
        IIm1ConnectionService GetIm1ConnectionService();
        IPrescriptionService GetPrescriptionService();
        ISessionService GetSessionService();
        ITokenValidationService GetTokenValidationService();
    }
}
