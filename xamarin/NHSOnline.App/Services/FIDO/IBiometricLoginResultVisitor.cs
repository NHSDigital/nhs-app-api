namespace NHSOnline.App.Services.FIDO
{
    internal interface IBiometricLoginResultVisitor<T>
    {
        T Visit(BiometricLoginResult.Authorised authorised);
        T Visit(BiometricLoginResult.Unauthorised unauthorised);
        T Visit(BiometricLoginResult.Failed failed);
        T Visit(BiometricLoginResult.UserCancelled userCancelled);
        T Visit(BiometricLoginResult.SystemCancelled systemCancelled);
        T Visit(BiometricLoginResult.NotRegistered notRegistered);
        T Visit(BiometricLoginResult.Invalidated invalidated);
    }
}