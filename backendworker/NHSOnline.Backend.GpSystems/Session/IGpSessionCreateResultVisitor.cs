namespace NHSOnline.Backend.GpSystems.Session
{
    public interface IGpSessionCreateResultVisitor<out T>
    {
        T Visit(GpSessionCreateResult.SuccessfullyCreated successfullyCreated);

        T Visit(GpSessionCreateResult.InvalidIm1ConnectionToken invalidIm1ConnectionToken);

        T Visit(GpSessionCreateResult.SupplierSystemUnavailable supplierSystemUnavailable);

        T Visit(GpSessionCreateResult.SupplierSystemBadResponse supplierSystemBadResponse);

        T Visit(GpSessionCreateResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader);

        T Visit(GpSessionCreateResult.InvalidUserCredentials invalidUserCredentials);
        
        T Visit(GpSessionCreateResult.InvalidRequest invalidRequest);

        T Visit(GpSessionCreateResult.UnknownError unknownError);
    }
}