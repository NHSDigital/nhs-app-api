using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    public class CourseResultAuditingVisitor : ICourseResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<CoursesController> _logger;
        private readonly FilteringCounts _coursesCount;

        private const string AuditType = AuditingOperations.RepeatPrescriptionsViewRepeatMedicationsResponse;

        public CourseResultAuditingVisitor(IAuditor auditor, ILogger<CoursesController> logger, FilteringCounts coursesCount)
        {
            _auditor = auditor;
            _logger = logger;
            _coursesCount = coursesCount;
        }

        public async Task Visit(GetCoursesResult.Success result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType,
                    "Courses successfully retrieved. " +
                    $"Total courses before filtering: {_coursesCount.ReceivedCount}, " +
                    $"Total courses returned after filtering: {_coursesCount.ReturnedCount}");
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
                await _auditor.PostOperationAudit(AuditType, "Error retrieving courses: Supplier Unavailable");
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
                await _auditor.PostOperationAudit(AuditType, "Error retrieving courses: Internal Server Error");
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
                await _auditor.PostOperationAudit(AuditType, "Error retrieving courses: Insufficient permissions");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetCoursesResult.Forbidden)}");
            }
        }
    }
}
