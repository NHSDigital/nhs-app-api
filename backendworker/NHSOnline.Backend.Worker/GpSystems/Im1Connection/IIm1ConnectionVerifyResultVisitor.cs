namespace NHSOnline.Backend.Worker.GpSystems.Im1Connection
{
    public interface IIm1ConnectionVerifyResultVisitor<out T>
    {
        T Visit(Im1ConnectionVerifyResult.SuccessfullyVerified result);

        T Visit(Im1ConnectionVerifyResult.InsufficientPermissions result);

        T Visit(Im1ConnectionVerifyResult.NotFound result);

        T Visit(Im1ConnectionVerifyResult.SupplierSystemUnavailable result);

        T Visit(Im1ConnectionVerifyResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader);

        T Visit(Im1ConnectionVerifyResult.InvalidUserCredentials invalidUserCredentials);

        T Visit(Im1ConnectionVerifyResult.InvalidRequest invalidRequest);

        T Visit(Im1ConnectionVerifyResult.UnknownError unknownError);
    }
}