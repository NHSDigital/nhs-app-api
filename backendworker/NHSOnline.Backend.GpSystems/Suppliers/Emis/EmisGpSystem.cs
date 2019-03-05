using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    [SuppressMessage("NDepend", "ND1400:AvoidNamespacesMutuallyDependent", 
        Justification = "GpSystem responsibility is pointing to concrete classes in child namespaces.")]
    public class EmisGpSystem : IGpSystem
    {
        private readonly IServiceProvider _serviceProvider;

        public EmisGpSystem(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Supplier Supplier => Supplier.Emis;

        public IAppointmentsService GetAppointmentsService()
        {
            return _serviceProvider.GetService<EmisAppointmentsService>();
        }

        public IAppointmentSlotsService GetAppointmentSlotsService()
        {
            return _serviceProvider.GetService<EmisAppointmentSlotsService>();
        }

        public ICourseService GetCourseService()
        {
            return _serviceProvider.GetService<EmisCourseService>();
        }

        public IDemographicsService GetDemographicsService()
        {
            return _serviceProvider.GetService<EmisDemographicsService>();
        }

        public IPatientRecordService GetPatientRecordService()
        {
            return _serviceProvider.GetService<EmisPatientRecordService>();
        }

        public IIm1ConnectionService GetIm1ConnectionService()
        {
            return _serviceProvider.GetService<EmisIm1ConnectionService>();
        }

        public IPrescriptionService GetPrescriptionService()
        {
            return _serviceProvider.GetService<EmisPrescriptionService>();
        }

        public IPrescriptionRequestValidationService GetPrescriptionRequestValidationService()
        {
            return _serviceProvider.GetService<EmisPrescriptionRequestValidationService>();
        }

        public ISessionService GetSessionService()
        {
            return (IEmisSessionService) _serviceProvider.GetService(typeof(IEmisSessionService));
        }

        public ISessionExtendService GetSessionExtendService()
        {
            return _serviceProvider.GetService<EmisSessionExtendService>();
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return _serviceProvider.GetService<EmisTokenValidationService>();
        }

        public ILinkageService GetLinkageService()
        {
            return _serviceProvider.GetService<EmisLinkageService>();
        }

        public ILinkageRequestValidationService GetLinkageRequestValidationService()
        {
            return _serviceProvider.GetService<EmisLinkageRequestValidationService>();
        }
    }
}