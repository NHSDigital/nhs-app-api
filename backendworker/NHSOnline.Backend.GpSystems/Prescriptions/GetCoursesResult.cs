using NHSOnline.Backend.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public abstract class GetCoursesResult
    {
        public abstract T Accept<T>(ICourseResultVisitor<T> visitor);

        public class Success : GetCoursesResult
        {
            public CourseListResponse Response { get; }
            
            public FilteringCounts FilteringCounts { get; }

            public bool? AllowFreeTextPrescriptions { get; }

            public Success(CourseListResponse response,
                FilteringCounts filteringCounts,
                bool? allowFreeTextPrescriptions = null)
            {
                Response = response;
                FilteringCounts = filteringCounts;
                AllowFreeTextPrescriptions = allowFreeTextPrescriptions;
            }

            public override T Accept<T>(ICourseResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : GetCoursesResult
        {
            public override T Accept<T>(ICourseResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : GetCoursesResult
        {
            public override T Accept<T>(ICourseResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadGateway : GetCoursesResult
        {
            public override T Accept<T>(ICourseResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
