using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    [Route("patient/courses"),PfsSecurityMode]
    public class CoursesController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<CoursesController> _logger;

        private readonly IAuditor _auditor;
        
        public CoursesController(
            ILogger<CoursesController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor)
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userSession = HttpContext.GetUserSession();

            _auditor.Audit(Constants.AuditingTitles.RepeatPrescriptionsViewRepeatMedicationsRequest, "Attempting to retrieve courses");
            _logger.LogInformation($"Fetching courses interface for supplier {userSession.Supplier}");
            
            var courseService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetCourseService();

            var result = await courseService.GetCourses(userSession);

            result.Accept(new CourseResultAuditingVisitor(_auditor));
            return result.Accept(new CourseResultVisitor());
        }
    }
}
