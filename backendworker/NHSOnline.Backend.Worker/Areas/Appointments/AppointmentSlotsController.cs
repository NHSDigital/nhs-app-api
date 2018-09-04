using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    [Route("patient/appointment-slots"),PfsSecurityMode]
    public class AppointmentSlotsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<AppointmentSlotsController> _logger;
        private readonly IAuditor _auditor;

        public AppointmentSlotsController(
            IGpSystemFactory gpSystemFactory,
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            ILogger<AppointmentSlotsController> logger,
            IAuditor auditor
        )
        {
            _gpSystemFactory = gpSystemFactory;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PatientAppointmentSlotsQueryParameters queryParameters)
        {
            try
            {
                _logger.LogEnter(nameof(Get));
                
                _auditor.Audit(Constants.AuditingTitles.GetSlotsAuditTypeRequest, "Attempting to get available appointments");

                if (!new DateRangeValidator(_dateTimeOffsetProvider).IsValid(queryParameters.FromDate,
                    queryParameters.ToDate))
                {
                    _logger.LogError(
                        $"Query parameters are invalid. From date: '{queryParameters.FromDate?.ToString("o", CultureInfo.InvariantCulture)}', To date '{queryParameters.ToDate?.ToString("o", CultureInfo.InvariantCulture)}'");
                    var badRequestResult = new AppointmentSlotsResult.BadRequest();
                    badRequestResult.Accept(new AppointmentSlotsAuditingVisitor(_auditor));

                    return badRequestResult.Accept(new AppointmentSlotsResultVisitor());
                }

                var userSession = HttpContext.GetUserSession();
                _logger.LogDebug($"Fetch Appointment Slots Service for GP System: '{userSession.Supplier}'.");
                var appointmentService = _gpSystemFactory.CreateGpSystem(userSession.Supplier)
                    .GetAppointmentSlotsService();

                var dateRange = new AppointmentSlotsDateRange(
                    _dateTimeOffsetProvider,
                    queryParameters.FromDate,
                    queryParameters.ToDate
                );
            
                var result = await appointmentService.GetSlots(userSession, dateRange);

                result.Accept(new AppointmentSlotsAuditingVisitor(_auditor));
                return result.Accept(new AppointmentSlotsResultVisitor());
            }
            finally
            {
                _logger.LogExit(nameof(Get));
            }
            
        }
    }
}
