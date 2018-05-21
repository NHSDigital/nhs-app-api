using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Router;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    [Route("patient/courses")]
    public class CoursesController : Controller
    {
        private readonly IBridgeFactory _bridgeFactory;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(
            ILoggerFactory loggerFactory,
            IBridgeFactory bridgeFactory)
        {
            _logger = loggerFactory.CreateLogger<CoursesController>();
            _bridgeFactory = bridgeFactory;
        }

        [HttpGet, TimeoutExceptionFilter]
        public async Task<IActionResult> Get()
        {
            UserSession userSession = HttpContext.GetUserSession();

            var courseService = _bridgeFactory
                .CreateBridge(userSession.Supplier)
                .GetCourseService();

            var result = await courseService.Get(userSession);

            return result.Accept(new CourseResultVisitor());
        }
    }
}
