using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using NHSOnline.Backend.Worker.Bridges.Emis.AppointmentSlots;
using NHSOnline.Backend.Worker.Bridges.Emis.Demographics;
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.Date;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Appointments;
using NHSOnline.Backend.Worker.Router.Demographics;
using NHSOnline.Backend.Worker.Router.Im1Connection;
using NHSOnline.Backend.Worker.Router.Prescriptions;
using NHSOnline.Backend.Worker.Router.MyRecord;
using NHSOnline.Backend.Worker.Router.Session;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisBridge : IBridge
    {
        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;
        private readonly IEmisAllergyMapper _emisAllergyMapper;
        private readonly IEmisDemographicsMapper _emisDemographicsMapper;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<ConfigurationSettings> _settings;

        public EmisBridge(
            ILoggerFactory loggerFactory,
            IEmisClient emisClient,
            IEmisPrescriptionMapper emisPrescriptionMapper,
            IEmisAllergyMapper emisAllergyMapper,
            IEmisDemographicsMapper emisDemographicsMapper,
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            IOptions<ConfigurationSettings> settings,
            IConfiguration configuration)
        {
            _emisClient = emisClient;
            _emisPrescriptionMapper = emisPrescriptionMapper;
            _emisAllergyMapper = emisAllergyMapper;
            _emisDemographicsMapper = emisDemographicsMapper;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _loggerFactory = loggerFactory;
            _settings = settings;
        }

        public SupplierEnum Supplier => SupplierEnum.Emis;

        public IAppointmentsService GetAppointmentsService()
        {
            return new EmisAppointmentsService(_emisClient, _loggerFactory);
        }

        public ICourseService GetCourseService()
        {
            return new EmisCourseService(_loggerFactory, _settings, _emisClient, _emisPrescriptionMapper);
        }
        
        public IDemographicsService GetDemographicsService()
        {
            return new EmisDemographicsService(_loggerFactory, _emisClient, _emisDemographicsMapper);
        }

        public IIm1ConnectionService GetIm1ConnectionService()
        {
            return new EmisIm1ConnectionService(_emisClient);
        }

        public IPrescriptionService GetPrescriptionService()
        {
            return new EmisPrescriptionService(_loggerFactory, _settings, _emisClient, _emisPrescriptionMapper);
        }

        public IPatientRecordService GetPatientRecordService()
        {
            return new EmisPatientRecordService(_loggerFactory, _settings, _emisClient, _emisAllergyMapper);
        }

        public ISessionService GetSessionService()
        {
            return new EmisSessionService(_emisClient, _settings);
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return new EmisTokenValidationService();
        }

        public IAppointmentSlotsService GetAppointmentSlotsService()
        {
            return new EmisAppointmentSlotsService(_emisClient, _loggerFactory, new AppointmentSlotsResponseMapper(_dateTimeOffsetProvider));
        }
    }
}
