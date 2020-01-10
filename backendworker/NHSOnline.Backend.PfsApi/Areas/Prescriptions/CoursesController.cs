using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    [ApiVersionRoute("patient/courses")]
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

            var coursesCount = new FilteringCounts();
            if (result is GetCoursesResult.Success successResult)
            {
                coursesCount = successResult.FilteringCounts;
                LogCourseInformation(coursesCount);
            }

            await result.Accept(new CourseResultAuditingVisitor(_auditor, _logger, coursesCount));
            return await result.Accept(new CourseResultVisitor(_sessionCacheService, _errorReferenceGenerator, userSession));
        }
        
        private void LogCourseInformation(FilteringCounts result) 
        {
            try
            {
                var kvp = new Dictionary<string, string>
                {
                    { "Courses Received", 
                        result.ReceivedCount.ToString(CultureInfo.InvariantCulture) },
                    { "Courses remaining after filtering out non-repeats", 
                        result.FilteredRemainingRepeatsCount.ToString(CultureInfo.InvariantCulture) },
                    { "Courses filtered out for exceeding maximum allowance", 
                        result.FilteredMaxAllowanceDiscardedCount.ToString(CultureInfo.InvariantCulture) },
                    { "Courses Returned", 
                        result.ReturnedCount.ToString(CultureInfo.InvariantCulture) }
                };
                
                _logger.LogInformationKeyValuePairs("Courses Count", kvp);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to log courses filtering details. " +
                                    "Catching exception to prevent inability to get courses");
            }
        }
    }
}
