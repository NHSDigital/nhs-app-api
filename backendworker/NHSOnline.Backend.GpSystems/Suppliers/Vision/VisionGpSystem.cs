using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    [SuppressMessage("NDepend", "ND1400:AvoidNamespacesMutuallyDependent",
        Justification = "GpSystem responsibility is pointing to concrete classes in child namespaces.")]
    public class VisionGpSystem : IGpSystem
    {
        private readonly IServiceProvider _serviceProvider;

        public Supplier Supplier => Supplier.Vision;

        public VisionGpSystem(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IAppointmentsService GetAppointmentsService()
        {
            return _serviceProvider.GetService<VisionAppointmentsService>();
        }

        public IAppointmentsValidationService GetAppointmentsValidationService()
        {
            return _serviceProvider.GetService<VisionAppointmentsValidationService>();
        }

        public IAppointmentSlotsService GetAppointmentSlotsService()
        {
            return _serviceProvider.GetService<VisionAppointmentSlotsService>();
        }

        public ICourseService GetCourseService()
        {
            return _serviceProvider.GetService<VisionCourseService>();
        }

        public IDemographicsService GetDemographicsService()
        {
            return _serviceProvider.GetService<VisionDemographicsService>();
        }

        public IIm1ConnectionService GetIm1ConnectionService()
        {
            return _serviceProvider.GetService<VisionIm1ConnectionService>();
        }

        public IPrescriptionService GetPrescriptionService()
        {
            return _serviceProvider.GetService<VisionPrescriptionService>();
        }

        public ISessionService GetSessionService()
        {
            return _serviceProvider.GetService<VisionSessionService>();
        }

        public ISessionExtendService GetSessionExtendService()
        {
            return _serviceProvider.GetService<VisionSessionExtendService>();
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return _serviceProvider.GetService<VisionTokenValidationService>();
        }

        public IPatientRecordService GetPatientRecordService()
        {
            return _serviceProvider.GetService<VisionPatientRecordService>();
        }
        
        public ILinkageService GetLinkageService()
        {
            return _serviceProvider.GetService<VisionLinkageService>();
        }
        
        public ILinkageValidationService GetLinkageValidationService()
        {
            return _serviceProvider.GetService<VisionLinkageValidationService>();
        }
        
        public IPrescriptionValidationService GetPrescriptionRequestValidationService()
        {
            return _serviceProvider.GetService<VisionPrescriptionValidationService>();
        }
    }
}