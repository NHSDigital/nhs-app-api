using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.CID.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisGpSystem : IGpSystem
    {
        private readonly IServiceProvider _serviceProvider;

        public EmisGpSystem(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public SupplierEnum Supplier => SupplierEnum.Emis;

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
            return _serviceProvider.GetService<EmisSessionService>();
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return _serviceProvider.GetService<EmisTokenValidationService>();
        }

        public ILinkageService GetLinkageService()
        {
            bool
                .TryParse(
                    Environment.GetEnvironmentVariable("EMIS_LINKAGE_USE_STUBBED_DATA"), 
                    out var useStubbedData);
            
            if (useStubbedData)
            {
                return _serviceProvider.GetService<CidLinkageService>();
            }
            else
            {
                return _serviceProvider.GetService<EmisLinkageService>();                                
            }
        }
    }
}
