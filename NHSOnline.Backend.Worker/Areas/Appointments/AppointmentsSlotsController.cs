using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Bridges.Emis.Appointments;
using NHSOnline.Backend.Worker.Date;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Appointments;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    [Route("patient/appointment-slots")]
    public class AppointmentsSlotsController : Controller
    {
        private readonly ISystemProviderFactory _systemProviderFactory;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<AppointmentsSlotsController> _logger;
        
        public AppointmentsSlotsController(
            ISystemProviderFactory systemProviderFactory,
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            ILoggerFactory loggerFactory
            )
        {
            _systemProviderFactory = systemProviderFactory;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = loggerFactory.CreateLogger<AppointmentsSlotsController>();
        }
        
        [HttpGet, TimeoutExceptionFilter]
        public async Task<IActionResult> Get([FromQuery] PatientAppointmentSlotsQueryParameters queryParameters)
        {
            if (!new DateRangeValidator().IsValid(queryParameters.FromDate, queryParameters.ToDate))
            {
                _logger
                    .LogError("Query parameters are invalid");
                return new AppointmentSlotsResult.BadRequest().Accept(new AppointmentSlotsResultVisitor());
            }
            
            var userSession = HttpContext.GetUserSession();
            var appointmentService = _systemProviderFactory.CreateSystemProvider(userSession.Supplier).GetAppointmentService(userSession);
            
            var dateRange = new AppointmentSlotDateRange(_dateTimeOffsetProvider, queryParameters.FromDate, queryParameters.ToDate);
            var result = await appointmentService.Get(dateRange.FromDate, dateRange.ToDate);

            return result.Accept(new AppointmentSlotsResultVisitor());
        }
    }
}
