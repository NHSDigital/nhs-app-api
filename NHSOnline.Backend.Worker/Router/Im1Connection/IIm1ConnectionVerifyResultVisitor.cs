namespace NHSOnline.Backend.Worker.Router.Im1Connection
{
    public interface IIm1ConnectionVerifyResultVisitor<out T>
    {
        T Visit(Im1ConnectionVerifyResult.SuccessfullyVerified result);
        T Visit(Im1ConnectionVerifyResult.InsufficientPermissions result);
        T Visit(Im1ConnectionVerifyResult.NotFound result);
        T Visit(Im1ConnectionVerifyResult.SupplierSystemUnavailable result);
    }
}