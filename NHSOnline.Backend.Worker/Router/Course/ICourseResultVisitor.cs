namespace NHSOnline.Backend.Worker.Router.Course
{
    public interface ICourseResultVisitor<out T>
    {
        T Visit(GetCoursesResult.SuccessfullyRetrieved result);
        T Visit(GetCoursesResult.Unsuccessful result);
    }
}
