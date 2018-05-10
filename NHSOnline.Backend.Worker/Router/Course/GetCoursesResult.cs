using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.Router.Course
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

        public class Unsuccessful : GetCoursesResult
        {
            public override T Accept<T>(ICourseResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
