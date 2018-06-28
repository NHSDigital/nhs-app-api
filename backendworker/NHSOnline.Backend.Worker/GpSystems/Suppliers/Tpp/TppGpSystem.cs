using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppGpSystem : IGpSystem
    {
        private readonly IServiceProvider _serviceProvider;

        public TppGpSystem(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public SupplierEnum Supplier => SupplierEnum.Tpp;

        public IAppointmentsService GetAppointmentsService()
        {
            throw new NotImplementedException();
        }

        public IAppointmentSlotsService GetAppointmentSlotsService()
        {
            throw new NotImplementedException();
        }

        public ICourseService GetCourseService()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public ISessionService GetSessionService()
        {
            return _serviceProvider.GetService<TppSessionService>();
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return _serviceProvider.GetService<TppTokenValidationService>();
        }
    }
}
