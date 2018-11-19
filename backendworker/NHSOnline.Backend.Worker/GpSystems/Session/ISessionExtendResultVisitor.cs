namespace NHSOnline.Backend.Worker.GpSystems.Session
{
    public interface ISessionExtendResultVisitor<out T>
    {
        T Visit(SessionExtendResult.SuccessfullyExtended successfullyExtended);

        T Visit(SessionExtendResult.SupplierSystemUnavailable supplierSystemUnavailable);
    }
}