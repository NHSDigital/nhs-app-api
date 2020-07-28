using System.Diagnostics.CodeAnalysis;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems
{
    [SuppressMessage("Microsoft.Naming", "CA1024", Justification = "Methods are needed to match interface definition.")]
    public class NullGpSystem : IGpSystem
    {
        public Supplier Supplier => Supplier.Disconnected;
        public bool SupportsLinkedAccounts => false;

        public IAppointmentsService GetAppointmentsService() => throw new NoGpSessionAvailableException();

        public IAppointmentSlotsService GetAppointmentSlotsService() => throw new NoGpSessionAvailableException();

        public IAppointmentsValidationService GetAppointmentsValidationService() => throw new NoGpSessionAvailableException();

        public ICourseService GetCourseService() => throw new NoGpSessionAvailableException();

        public IDemographicsService GetDemographicsService() => throw new NoGpSessionAvailableException();

        public IIm1ConnectionService GetIm1ConnectionService() => throw new NoGpSessionAvailableException();

        public IPrescriptionService GetPrescriptionService() => throw new NoGpSessionAvailableException();

        public IPrescriptionValidationService GetPrescriptionValidationService() => throw new NoGpSessionAvailableException();

        public ISessionService GetSessionService() => throw new NoGpSessionAvailableException();

        public ISessionExtendService GetSessionExtendService() => throw new NoGpSessionAvailableException();

        public ITokenValidationService GetTokenValidationService() => throw new NoGpSessionAvailableException();

        public IPatientRecordService GetPatientRecordService() => throw new NoGpSessionAvailableException();

        public ILinkageService GetLinkageService() => throw new NoGpSessionAvailableException();

        public ILinkageValidationService GetLinkageValidationService() => throw new NoGpSessionAvailableException();

        public ILinkedAccountsService GetLinkedAccountsService() => throw new NoGpSessionAvailableException();

        public IPatientMessagesService GetPatientMessagesService() => throw new NoGpSessionAvailableException();

        public IRecreateSessionMapperService GetRecreateSessionMapperService() => throw new NoGpSessionAvailableException();
    }
}