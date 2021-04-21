namespace NHSOnline.App.DependencyServices.Biometrics
{
    public interface IBiometricAuthVerifyUserResultVisitor<T>
    {
        T Visit(BiometricAuthVerifyUserResult.Authorised authorised);
        T Visit(BiometricAuthVerifyUserResult.UserCancelled userCancelled);
        T Visit(BiometricAuthVerifyUserResult.SystemCancelled systemCancelled);
        T Visit(BiometricAuthVerifyUserResult.Unauthorised unauthorised);
        T Visit(BiometricAuthVerifyUserResult.LockedOut lockedOut);
    }
}