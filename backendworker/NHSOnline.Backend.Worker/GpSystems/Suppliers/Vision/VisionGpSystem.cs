using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
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
        
        public ILinkageRequestValidationService GetLinkageRequestValidationService()
        {
            return _serviceProvider.GetService<VisionLinkageRequestValidationService>();
        }
        
        public IPrescriptionRequestValidationService GetPrescriptionRequestValidationService()
        {
            return _serviceProvider.GetService<VisionPrescriptionRequestValidationService>();
        }
    }
}