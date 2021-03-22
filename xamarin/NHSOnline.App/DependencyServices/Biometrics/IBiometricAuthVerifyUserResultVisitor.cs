namespace NHSOnline.App.DependencyServices.Biometrics
{
    public interface IBiometricAuthVerifyUserResultVisitor<T>
    {
        T Visit(BiometricAuthVerifyUserResult.Authorised authorised);
        T Visit(BiometricAuthVerifyUserResult.Cancelled cancelled);
        T Visit(BiometricAuthVerifyUserResult.Failed failed);
    }
}