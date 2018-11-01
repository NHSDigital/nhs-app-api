using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using System.Diagnostics.CodeAnalysis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest
{
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
            throw new NotImplementedException();
        }

        public ILinkageRequestValidationService GetLinkageRequestValidationService()
        {
            throw new NotImplementedException();
        }

        public ILinkageService GetLinkageService()
        {
            throw new NotImplementedException();
        }

        public IPatientRecordService GetPatientRecordService()
        {
            throw new NotImplementedException();
        }

        public IPrescriptionRequestValidationService GetPrescriptionRequestValidationService()
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