namespace NHSOnline.Backend.Worker.Router.Prescriptions
{
    public interface ICourseResultVisitor<out T>
    {
        T Visit(GetCoursesResult.SuccessfullyRetrieved result);
        T Visit(GetCoursesResult.Unsuccessful result);
        T Visit(GetCoursesResult.SupplierBadData result);
    }
}
