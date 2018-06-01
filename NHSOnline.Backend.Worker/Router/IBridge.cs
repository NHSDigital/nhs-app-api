using NHSOnline.Backend.Worker.Router.Appointments;
using NHSOnline.Backend.Worker.Router.Demographics;
using NHSOnline.Backend.Worker.Router.Im1Connection;
using NHSOnline.Backend.Worker.Router.MyRecord;
using NHSOnline.Backend.Worker.Router.Prescriptions;
using NHSOnline.Backend.Worker.Router.Session;

namespace NHSOnline.Backend.Worker.Router
{
    public interface IBridge
    {
        SupplierEnum Supplier { get; }

        IAppointmentsService GetAppointmentsService();
        IAppointmentSlotsService GetAppointmentSlotsService();
        ICourseService GetCourseService();
        IDemographicsService GetDemographicsService();
        IIm1ConnectionService GetIm1ConnectionService();
        IPrescriptionService GetPrescriptionService();
        ISessionService GetSessionService();
        ITokenValidationService GetTokenValidationService();
        IPrescriptionService GetPrescriptionService();
        ICourseService GetCourseService();
        IAppointmentSlotsService GetAppointmentSlotsService();
        IPatientRecordService GetPatientRecordService();
    }
}
