namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface ICourseResultVisitor<out T>
    {
        T Visit(GetCoursesResult.SuccessfullyRetrieved result);
        T Visit(GetCoursesResult.SupplierSystemUnavailable result);
        T Visit(GetCoursesResult.InternalServerError result);
        T Visit(GetCoursesResult.SupplierNotEnabled result);
    }
}
