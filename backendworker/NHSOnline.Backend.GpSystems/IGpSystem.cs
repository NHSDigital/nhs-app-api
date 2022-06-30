using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Messages;

namespace NHSOnline.Backend.GpSystems
{
    public interface IGpSystem
    {
        Supplier Supplier { get; }

        IAppointmentSlotsService GetAppointmentSlotsService();

        IAppointmentsService GetAppointmentsService();

        IAppointmentsValidationService GetAppointmentsValidationService();

        ICourseService GetCourseService();

        IDemographicsService GetDemographicsService();

        IIm1ConnectionService GetIm1ConnectionService();

        ILinkageService GetLinkageService();

        ILinkageValidationService GetLinkageValidationService();

        ILinkedAccountsService GetLinkedAccountsService();

        IPatientMessagesService GetPatientMessagesService();

        IPatientRecordService GetPatientRecordService();

        IPrescriptionService GetPrescriptionService();

        IPrescriptionValidationService GetPrescriptionValidationService();

        IRecreateSessionMapperService GetRecreateSessionMapperService();

        ISessionExtendService GetSessionExtendService();

        ISessionService GetSessionService();

        ITokenValidationService GetTokenValidationService();

        bool SupportsLinkedAccounts { get; }

        int PrescriptionSpecialRequestCharacterLimit => Constants.SpecialRequestCharacterLimit.FrontendLimit;

        int AppointmentBookingReasonCharacterLimit => Constants.BookingReasonCharacterLimit.FrontendLimit;
    }
}
