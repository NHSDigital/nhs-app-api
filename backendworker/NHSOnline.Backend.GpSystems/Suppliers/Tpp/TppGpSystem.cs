using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    [SuppressMessage("NDepend", "ND1400:AvoidNamespacesMutuallyDependent",
        Justification = "GpSystem responsibility is pointing to concrete classes in child namespaces.")]
    [SuppressMessage("Microsoft.Naming", "CA1024", Justification = "Methods are needed to match interface definition.")]
    public class TppGpSystem : IGpSystem
    {
        private readonly IServiceProvider _serviceProvider;

        public TppGpSystem(IServiceProvider serviceProvider, TppConfigurationSettings configurationSettings)
        {
            _serviceProvider = serviceProvider;
            SupportsLinkedAccounts = configurationSettings.SupportsLinkedAccounts;
        }

        public Supplier Supplier => Supplier.Tpp;

        public IAppointmentsService GetAppointmentsService()
        {
            return _serviceProvider.GetService<TppAppointmentsService>();
        }

        public IAppointmentsValidationService GetAppointmentsValidationService()
        {
            return _serviceProvider.GetService<TppAppointmentsValidationService>();
        }

        public IAppointmentSlotsService GetAppointmentSlotsService()
        {
            return _serviceProvider.GetService<TppAppointmentSlotsService>();
        }

        public ICourseService GetCourseService()
        {
            return _serviceProvider.GetService<TppCourseService>();
        }

        public IDemographicsService GetDemographicsService()
        {
            return _serviceProvider.GetService<TppDemographicsService>();
        }

        public IPatientRecordService GetPatientRecordService()
        {
            return _serviceProvider.GetService<TppPatientRecordService>();
        }

        public IIm1ConnectionService GetIm1ConnectionService()
        {
            return _serviceProvider.GetService<TppIm1ConnectionService>();
        }

        public IPrescriptionService GetPrescriptionService()
        {
            return _serviceProvider.GetService<TppPrescriptionService>();
        }

        public IPrescriptionValidationService GetPrescriptionValidationService()
        {
            return _serviceProvider.GetService<TppPrescriptionValidationService>();
        }

        public ISessionService GetSessionService()
        {
            return _serviceProvider.GetService<TppSessionService>();
        }

        public ISessionExtendService GetSessionExtendService()
        {
            return _serviceProvider.GetService<TppSessionExtendService>();
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return _serviceProvider.GetService<TppTokenValidationService>();
        }

        public ILinkageService GetLinkageService()
        {
            return _serviceProvider.GetService<TppLinkageService>();
        }

        public ILinkageValidationService GetLinkageValidationService()
        {
            return _serviceProvider.GetService<TppLinkageValidationService>();
        }

        public ILinkedAccountsService GetLinkedAccountsService()
        {
            return _serviceProvider.GetService<TppLinkedAccountsService>();
        }

        public IPatientMessagesService GetPatientMessagesService()
        {
            return _serviceProvider.GetService<TppPatientMessagesService>();
        }

        public IRecreateSessionMapperService GetRecreateSessionMapperService()
        {
            return _serviceProvider.GetService<TppRecreateSessionMapperService>();
        }

        public bool SupportsLinkedAccounts { get; }

        public int PrescriptionSpecialRequestCharacterLimit { get; } = Constants.SpecialRequestCharacterLimit.Tpp;
    }
}
