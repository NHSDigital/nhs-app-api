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
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Linkage;
using NHSOnline.Backend.Support;

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
            throw new NotImplementedException();
        }

        public IDemographicsService GetDemographicsService()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IPrescriptionValidationService GetPrescriptionRequestValidationService()
        {
            throw new NotImplementedException();
        }

        public IPrescriptionService GetPrescriptionService()
        {
            throw new NotImplementedException();
        }

        public ISessionExtendService GetSessionExtendService()
        {
            throw new NotImplementedException();
        }

        public ISessionService GetSessionService()
        {
            return _serviceProvider.GetService<MicrotestSessionService>();
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return _serviceProvider.GetService<MicrotestTokenValidationService>();
        }
    }
}