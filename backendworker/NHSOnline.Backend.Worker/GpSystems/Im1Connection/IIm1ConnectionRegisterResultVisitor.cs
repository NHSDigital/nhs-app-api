namespace NHSOnline.Backend.Worker.GpSystems.Im1Connection
{
    public interface IIm1ConnectionRegisterResultVisitor<out T>
    {
        T Visit(Im1ConnectionRegisterResult.SuccessfullyRegistered result);
        T Visit(Im1ConnectionRegisterResult.InsufficientPermissions result);
        T Visit(Im1ConnectionRegisterResult.NotFound result);
        T Visit(Im1ConnectionRegisterResult.AccountAlreadyExists result);
        T Visit(Im1ConnectionRegisterResult.SupplierSystemUnavailable result);
    }
}