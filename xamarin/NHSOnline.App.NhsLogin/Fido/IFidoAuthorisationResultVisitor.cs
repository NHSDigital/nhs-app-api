namespace NHSOnline.App.NhsLogin.Fido
{
    public interface IFidoAuthorisationResultVisitor<T>
    {
        T Visit(FidoAuthorisationResult.Authorised authorised);
        T Visit(FidoAuthorisationResult.Unauthorised unauthorised);
        T Visit(FidoAuthorisationResult.PermanentLockout permanentLockout);
    }
}