using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    [Route("patient/courses")]
    public class CoursesController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<CoursesController> _logger;
        private readonly IAuditor _auditor;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        public CoursesController(
            ILogger<CoursesController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor,
            ISessionCacheService sessionCacheService,
            IErrorReferenceGenerator errorReferenceGenerator)
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
            _sessionCacheService = sessionCacheService;
            _errorReferenceGenerator = errorReferenceGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader(Name=PatientId)] Guid patientId)
        {
            var userSession = HttpContext.GetUserSession();
            
            var gpLinkedAccountUserSession = new GpLinkedAccountModel(
                userSession.GpUserSession, patientId
            );

            await _auditor.Audit(AuditingOperations.RepeatPrescriptionsViewRepeatMedicationsRequest, "Attempting to retrieve courses");
            _logger.LogInformation($"Fetching courses interface for supplier {userSession.GpUserSession.Supplier}");
            
            var courseService = _gpSystemFactory
                .CreateGpSystem(gpLinkedAccountUserSession.GpUserSession.Supplier)
                .GetCourseService();

            var result = await courseService.GetCourses(gpLinkedAccountUserSession);
            
            await result.Accept(new CourseResultAuditingVisitor(_auditor, _logger));
            return await result.Accept(new CourseResultVisitor(_sessionCacheService, _errorReferenceGenerator, userSession));
        }
    }
}
