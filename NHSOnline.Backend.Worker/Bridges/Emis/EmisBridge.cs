using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Appointments;
using NHSOnline.Backend.Worker.Router.Demographics;
using NHSOnline.Backend.Worker.Router.Im1Connection;
using NHSOnline.Backend.Worker.Router.Prescriptions;
using NHSOnline.Backend.Worker.Router.Session;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisBridge : IBridge
    {
        private readonly IServiceProvider _serviceProvider;

        public EmisBridge(IServiceProvider serviceProvider)
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

        public IIm1ConnectionService GetIm1ConnectionService()
        {
            return _serviceProvider.GetService<EmisIm1ConnectionService>();
        }

        public IPrescriptionService GetPrescriptionService()
        {
            return _serviceProvider.GetService<EmisPrescriptionService>();
        }

        public ISessionService GetSessionService()
        {
            return _serviceProvider.GetService<EmisSessionService>();
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return _serviceProvider.GetService<EmisTokenValidationService>();
        }
    }
}
