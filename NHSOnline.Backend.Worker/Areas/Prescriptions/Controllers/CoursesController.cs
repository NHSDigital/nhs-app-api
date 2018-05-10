using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions.Controllers
{
    [Route("patient/courses")]
    public class CoursesController : Controller
    {
        private readonly ISystemProviderFactory _systemProviderFactory;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(
            ILoggerFactory loggerFactory,
            ISystemProviderFactory systemProviderFactory)
        {
            _logger = loggerFactory.CreateLogger<CoursesController>();
            _systemProviderFactory = systemProviderFactory;
        }

        [HttpGet, TimeoutExceptionFilter]
        public async Task<IActionResult> Get()
        {
            UserSession userSession = HttpContext.GetUserSession();

            var courseService = _systemProviderFactory
                .CreateSystemProvider(userSession.Supplier)
                .GetCourseService();

            var result = await courseService.Get(userSession);

            return result.Accept(new CourseResultVisitor());
        }
    }
}
