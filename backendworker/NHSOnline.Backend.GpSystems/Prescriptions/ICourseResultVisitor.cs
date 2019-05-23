namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface ICourseResultVisitor<out T>
    {
        T Visit(GetCoursesResult.Success result);
        T Visit(GetCoursesResult.BadGateway result);
        T Visit(GetCoursesResult.InternalServerError result);
        T Visit(GetCoursesResult.Forbidden result);
    }
}
