using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Prescriptions;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    public class CourseResultAuditingVisitor : ICourseResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<CoursesController> _logger;
        
        public CourseResultAuditingVisitor(IAuditor auditor, ILogger<CoursesController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        private const string AuditType = AuditingOperations.RepeatPrescriptionsViewRepeatMedicationsResponse;
        
        public async Task Visit(GetCoursesResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Courses successfully retrieved - {0} courses",
                    result.Response?.Courses?.Count());
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetCoursesResult.Success)}");
            }
        }

        public async Task Visit(GetCoursesResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving courses: Supplier Unavailable");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetCoursesResult.BadGateway)}");
            }
        }

        public async Task Visit(GetCoursesResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving courses: Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetCoursesResult.InternalServerError)}");
            }
        }

        public async Task Visit(GetCoursesResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving courses: Insufficient permissions");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetCoursesResult.Forbidden)}");
            }
        }
    }
}