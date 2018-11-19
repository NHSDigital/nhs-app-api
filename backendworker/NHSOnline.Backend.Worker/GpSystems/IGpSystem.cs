using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems
{
    public interface IGpSystem
    {
        Supplier Supplier { get; }

        IAppointmentsService GetAppointmentsService();

        IAppointmentSlotsService GetAppointmentSlotsService();

        ICourseService GetCourseService();

        IDemographicsService GetDemographicsService();

        IIm1ConnectionService GetIm1ConnectionService();

        IPrescriptionService GetPrescriptionService();

        IPrescriptionRequestValidationService GetPrescriptionRequestValidationService();

        ISessionService GetSessionService();

        ISessionExtendService GetSessionExtendService();

        ITokenValidationService GetTokenValidationService();

        IPatientRecordService GetPatientRecordService();

        ILinkageService GetLinkageService();

        ILinkageRequestValidationService GetLinkageRequestValidationService();
    }
}
