using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppGpSystem : IGpSystem
    {
        private readonly IServiceProvider _serviceProvider;

        public TppGpSystem(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Supplier Supplier => Supplier.Tpp;

        public IAppointmentsService GetAppointmentsService()
        {
            return _serviceProvider.GetService<TppAppointmentsService>();
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

        public IPrescriptionRequestValidationService GetPrescriptionRequestValidationService()
        {
            return _serviceProvider.GetService<TppPrescriptionRequestValidationService>();
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

        public ILinkageRequestValidationService GetLinkageRequestValidationService()
        {
            return _serviceProvider.GetService<TppLinkageRequestValidationService>();
        }
    }
}
