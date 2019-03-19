using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    [Route("patient/courses")]
    public class CoursesController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<CoursesController> _logger;
        private readonly IAuditor _auditor;
        private readonly ISessionCacheService _sessionCacheService;

        public CoursesController(
            ILogger<CoursesController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor,
            ISessionCacheService sessionCacheService)
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
            _sessionCacheService = sessionCacheService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userSession = HttpContext.GetUserSession();

            await _auditor.Audit(Constants.AuditingTitles.RepeatPrescriptionsViewRepeatMedicationsRequest, "Attempting to retrieve courses");
            _logger.LogInformation($"Fetching courses interface for supplier {userSession.GpUserSession.Supplier}");
            
            var courseService = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetCourseService();

            var result = await courseService.GetCourses(userSession.GpUserSession);
            
            await result.Accept(new CourseResultAuditingVisitor(_auditor, _logger));
            return await result.Accept(new CourseResultVisitor(_sessionCacheService, userSession));
        }
    }
}
