using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Bridges.Emis.AppointmentSlots;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Appointments;
using NHSOnline.Backend.Worker.Support.Date;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    [Route("patient/appointment-slots")]
    public class AppointmentSlotsController : Controller
    {
        private readonly IBridgeFactory _bridgeFactory;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<AppointmentSlotsController> _logger;
        
        public AppointmentSlotsController(
            IBridgeFactory bridgeFactory,
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            ILoggerFactory loggerFactory
            )
        {
            _bridgeFactory = bridgeFactory;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = loggerFactory.CreateLogger<AppointmentSlotsController>();
        }
        
        [HttpGet, TimeoutExceptionFilter]
        public async Task<IActionResult> Get([FromQuery] PatientAppointmentSlotsQueryParameters queryParameters)
        {
            if (!new DateRangeValidator(_dateTimeOffsetProvider).IsValid(queryParameters.FromDate, queryParameters.ToDate))
            {
                _logger
                    .LogError("Query parameters are invalid");
                return new AppointmentSlotsResult.BadRequest().Accept(new AppointmentSlotsResultVisitor());
            }
            
            var userSession = HttpContext.GetUserSession();
            var appointmentService = _bridgeFactory.CreateBridge(userSession.Supplier).GetAppointmentSlotsService();
            
            var dateRange = new AppointmentSlotsDateRange(_dateTimeOffsetProvider, queryParameters.FromDate, queryParameters.ToDate);
            var result = await appointmentService.Get(userSession, dateRange.FromDate, dateRange.ToDate);

            return result.Accept(new AppointmentSlotsResultVisitor());
        }
    }
}
