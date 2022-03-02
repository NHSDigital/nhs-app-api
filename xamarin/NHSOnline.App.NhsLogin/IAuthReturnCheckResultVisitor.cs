namespace NHSOnline.App.NhsLogin
{
    public interface IAuthReturnCheckResultVisitor<T>
    {
        T Visit(AuthReturnCheckResult.Authorised authorised);
        T Visit(AuthReturnCheckResult.TermsAndConditionsDeclined termsDeclined);
        T Visit(AuthReturnCheckResult.SignatureInvalid signatureInvalid);
        T Visit(AuthReturnCheckResult.Failed failed);
    }
}