namespace NHSOnline.Backend.GpSystems.Session
{
    public interface ISessionLogoffResultVisitor<out T>
    {
        T Visit(SessionLogoffResult.SuccessfullyDeleted successfullyDeleted);

        T Visit(SessionLogoffResult.NotAuthenticated notAuthenticated);

        T Visit(SessionLogoffResult.SupplierSystemUnavailable supplierSystemUnavailable);
    }
}