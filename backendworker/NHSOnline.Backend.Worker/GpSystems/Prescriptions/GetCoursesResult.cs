using NHSOnline.Backend.Worker.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Prescriptions
{
    public abstract class GetCoursesResult
    {
        public abstract T Accept<T>(ICourseResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : GetCoursesResult
        {
            public CourseListResponse Response { get; }

            public SuccessfullyRetrieved(CourseListResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ICourseResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierNotEnabled : GetCoursesResult
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
        
        public class SupplierSystemUnavailable : GetCoursesResult
        {
            public override T Accept<T>(ICourseResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
