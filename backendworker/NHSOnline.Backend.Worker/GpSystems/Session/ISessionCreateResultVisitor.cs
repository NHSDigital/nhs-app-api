namespace NHSOnline.Backend.Worker.GpSystems.Session
{
    public interface ISessionCreateResultVisitor<out T>
    {
        T Visit(SessionCreateResult.SuccessfullyCreated successfullyCreated);

        T Visit(SessionCreateResult.InvalidIm1ConnectionToken invalidIm1ConnectionToken);

        T Visit(SessionCreateResult.SupplierSystemUnavailable supplierSystemUnavailable);

        T Visit(SessionCreateResult.SupplierSystemBadResponse supplierSystemBadResponse);

        T Visit(SessionCreateResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader);

        T Visit(SessionCreateResult.InvalidUserCredentials invalidUserCredentials);
        
        T Visit(SessionCreateResult.InvalidRequest invalidRequest);

        T Visit(SessionCreateResult.UnknownError unknownError);
    }
}