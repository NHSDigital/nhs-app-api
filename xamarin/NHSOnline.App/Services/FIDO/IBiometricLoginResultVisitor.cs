namespace NHSOnline.App.Services.FIDO
{
    internal interface IBiometricLoginResultVisitor<T>
    {
        T Visit(BiometricLoginResult.LoggedIn loggedIn);
        T Visit(BiometricLoginResult.Cancelled cancelled);
        T Visit(BiometricLoginResult.Failed failed);
        T Visit(BiometricLoginResult.NotRegistered notRegistered);
        T Visit(BiometricLoginResult.Invalidated invalidated);
    }
}