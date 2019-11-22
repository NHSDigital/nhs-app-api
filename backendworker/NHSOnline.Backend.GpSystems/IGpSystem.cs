using System;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.GpSystems.LinkedAccounts;

namespace NHSOnline.Backend.GpSystems
{
    public interface IGpSystem
    {
        Supplier Supplier { get; }

        IAppointmentsService GetAppointmentsService();

        IAppointmentSlotsService GetAppointmentSlotsService();

        IAppointmentsValidationService GetAppointmentsValidationService();

        ICourseService GetCourseService();

        IDemographicsService GetDemographicsService();

        IIm1ConnectionService GetIm1ConnectionService();

        IPrescriptionService GetPrescriptionService();

        IPrescriptionValidationService GetPrescriptionValidationService();

        ISessionService GetSessionService();

        ISessionExtendService GetSessionExtendService();

        ITokenValidationService GetTokenValidationService();

        IPatientRecordService GetPatientRecordService();

        ILinkageService GetLinkageService();

        ILinkageValidationService GetLinkageValidationService();

        ILinkedAccountsService GetLinkedAccountsService();

        Boolean SupportsLinkedAccounts { get; }
    }
}
