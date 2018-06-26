using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    [Route("patient/courses")]
    public class CoursesController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory)
        {
            _logger = loggerFactory.CreateLogger<CoursesController>();
            _gpSystemFactory = gpSystemFactory;
        }

        [HttpGet, TimeoutExceptionFilter]
        public async Task<IActionResult> Get()
        {
            UserSession userSession = HttpContext.GetUserSession();

            _logger.LogInformation($"Fetching courses interface for supplier {userSession.Supplier}");
            var courseService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetCourseService();

            var result = await courseService.Get(userSession);

            return result.Accept(new CourseResultVisitor());
        }
    }
}
