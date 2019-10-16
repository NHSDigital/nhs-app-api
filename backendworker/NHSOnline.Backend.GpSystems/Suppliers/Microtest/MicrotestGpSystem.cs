using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Linkage;
using System.Diagnostics.CodeAnalysis;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Prescriptions;
using NHSOnline.Backend.GpSystems.LinkedAccounts;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    [SuppressMessage("NDepend", "ND1400:AvoidNamespacesMutuallyDependent",
        Justification = "GpSystem responsibility is pointing to concrete classes in child namespaces.")]
    [SuppressMessage("Microsoft.Naming", "CA1024", Justification = "Methods are needed to match interface definition.")]
    public class MicrotestGpSystem : IGpSystem
    {
        private readonly IServiceProvider _serviceProvider;

        public MicrotestGpSystem(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Supplier Supplier => Supplier.Microtest;

        
        public IAppointmentSlotsService GetAppointmentSlotsService()
        {
            return _serviceProvider.GetService<MicrotestAppointmentSlotsService>();
        }

        public IAppointmentsService GetAppointmentsService()
        {
            return _serviceProvider.GetService<MicrotestAppointmentsService>();
        }

        public IAppointmentsValidationService GetAppointmentsValidationService()
        {
            return _serviceProvider.GetService<MicrotestAppointmentsValidationService>();
        }

        public ICourseService GetCourseService()
        {
            return _serviceProvider.GetService<MicrotestCourseService>();
        }

        public IDemographicsService GetDemographicsService()
        {
            return _serviceProvider.GetService<IMicrotestDemographicsService>();
        }

        public IIm1ConnectionService GetIm1ConnectionService()
        {
            return _serviceProvider.GetService<MicrotestIm1ConnectionService>();
        }

        public ILinkageValidationService GetLinkageValidationService()
        {
            return _serviceProvider.GetService<MicrotestLinkageValidationService>();
        }

        public ILinkageService GetLinkageService()
        {
            return _serviceProvider.GetService<MicrotestLinkageService>();
        }
        
        public IPatientRecordService GetPatientRecordService()
        {
            return _serviceProvider.GetService<MicrotestPatientRecordService>();
        }

        public IPrescriptionValidationService GetPrescriptionRequestValidationService()
        {
            return _serviceProvider.GetService<MicrotestPrescriptionValidationService>();
        }

        public IPrescriptionService GetPrescriptionService()
        {
            return _serviceProvider.GetService<MicrotestPrescriptionService>();
        }

        public ISessionExtendService GetSessionExtendService()
        {
            return _serviceProvider.GetService<MicrotestSessionExtendService>();
        }

        public ISessionService GetSessionService()
        {
            return _serviceProvider.GetService<MicrotestSessionService>();
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return _serviceProvider.GetService<MicrotestTokenValidationService>();
        }

        public ILinkedAccountsService GetLinkedAccountsService()
        {
            throw new NotImplementedException();
        }
    }
}