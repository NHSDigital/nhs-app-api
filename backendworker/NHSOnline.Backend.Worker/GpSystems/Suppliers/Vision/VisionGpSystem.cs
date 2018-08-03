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
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class VisionGpSystem : IGpSystem
    {
        private readonly IServiceProvider _serviceProvider;

        public Supplier Supplier => Supplier.Vision;

        public VisionGpSystem(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [SuppressMessage("Microsoft.Naming", "CA1024", 
            Justification = "Remove this suppression when method has been implemented.")]
        public IAppointmentsService GetAppointmentsService()
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Naming", "CA1024", 
            Justification = "Remove this suppression when method has been implemented.")]
        public IAppointmentSlotsService GetAppointmentSlotsService()
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Naming", "CA1024", 
            Justification = "Remove this suppression when method has been implemented.")]
        public ICourseService GetCourseService()
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Naming", "CA1024", 
            Justification = "Remove this suppression when method has been implemented.")]
        public IDemographicsService GetDemographicsService()
        {
            throw new NotImplementedException();
        }

        public IIm1ConnectionService GetIm1ConnectionService()
        {
            return _serviceProvider.GetService<VisionIm1ConnectionService>();
        }

        [SuppressMessage("Microsoft.Naming", "CA1024", 
            Justification = "Remove this suppression when method has been implemented.")]
        public IPrescriptionService GetPrescriptionService()
        {
            throw new NotImplementedException();
        }

        public ISessionService GetSessionService()
        {
            return _serviceProvider.GetService<VisionSessionService>();
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return _serviceProvider.GetService<VisionTokenValidationService>();
        }

        public IPatientRecordService GetPatientRecordService()
        {
            return _serviceProvider.GetService<VisionPatientRecordService>();
        }

        [SuppressMessage("Microsoft.Naming", "CA1024", 
            Justification = "Remove this suppression when method has been implemented.")]
        public ILinkageService GetLinkageService()
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Naming", "CA1024", 
            Justification = "Remove this suppression when method has been implemented.")]
        public IPrescriptionRequestValidationService GetPrescriptionRequestValidationService()
        {
            throw new NotImplementedException();
        }
    }
}