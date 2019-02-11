using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    public class CourseResultAuditingVisitor : ICourseResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        
        public CourseResultAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }

        private const string AuditType = Constants.AuditingTitles.RepeatPrescriptionsViewRepeatMedicationsResponse;
        
        public object Visit(GetCoursesResult.SuccessfullyRetrieved result)
        {
            _auditor.Audit(AuditType, $"Courses successfully retrieved - { result.Response?.Courses?.Count() } courses");
            return null;
        }

        public object Visit(GetCoursesResult.SupplierSystemUnavailable result)
        {
            _auditor.Audit(AuditType, "Error retrieving courses: Supplier Unavailable");
            return null;
        }

        public object Visit(GetCoursesResult.InternalServerError result)
        {
            _auditor.Audit(AuditType, "Error retrieving courses: Internal Server Error");
            return null;
        }

        public object Visit(GetCoursesResult.SupplierNotEnabled result)
        {
            _auditor.Audit(AuditType, "Error retrieving courses: Supplier Not Enabled");
            return null;
        }
    }
}