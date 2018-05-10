using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.Date;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Appointment;
using NHSOnline.Backend.Worker.Router.Appointments;
using NHSOnline.Backend.Worker.Router.Im1Connection;
using NHSOnline.Backend.Worker.Router.Session;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisSystemProvider : ISystemProvider
    {
        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILoggerFactory _loggerFactory;

        public EmisSystemProvider(ILoggerFactory loggerFactory, IEmisClient emisClient,
            IEmisPrescriptionMapper emisPrescriptionMapper, IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _emisClient = emisClient;
            _emisPrescriptionMapper = emisPrescriptionMapper;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _loggerFactory = loggerFactory;
        }

        public SupplierEnum Supplier => SupplierEnum.Emis;

        public IAppointmentsService GetAppointmentsService()
        {
            throw new System.NotImplementedException();
        }

        public ICourseService GetCourseService()
        {
            return new EmisCourseService(_loggerFactory, _emisClient, _emisPrescriptionMapper);
        }

        public IIm1ConnectionService GetIm1ConnectionService()
        {
            return new EmisIm1ConnectionService(_emisClient);
        }

        public IPrescriptionService GetPrescriptionService()
        {
            return new EmisPrescriptionService(_loggerFactory, _emisClient, _emisPrescriptionMapper);
        }

        public ISessionService GetSessionService()
        {
            return new EmisSessionService(_emisClient);
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return new EmisTokenValidationService();
        }

        public IAppointmentSlotsService GetAppointmentService(UserSession userSession)
        {
            return new EmisAppointmentSlotsService(userSession, _emisClient, _loggerFactory,_dateTimeOffsetProvider);
        }
    }
}
