namespace NHSOnline.Backend.Worker.Router.Session
{
    public interface ISessionCreateResultVisitor<out T>
    {
        T Visit(SessionCreateResult.SuccessfullyCreated successfullyCreated);
        T Visit(SessionCreateResult.InvalidIm1ConnectionToken successfullyCreated);
        T Visit(SessionCreateResult.SupplierSystemUnavailable successfullyCreated);
    }
}