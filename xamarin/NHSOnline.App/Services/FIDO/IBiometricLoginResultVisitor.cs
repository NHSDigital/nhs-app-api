namespace NHSOnline.App.Services.FIDO
{
    internal interface IBiometricLoginResultVisitor<T>
    {
        T Visit(BiometricLoginResult.Authorised authorised);
        T Visit(BiometricLoginResult.Failed failed);
        T Visit(BiometricLoginResult.Lockout lockout);
        T Visit(BiometricLoginResult.LegacySensorNotValid legacySensorNotValid);
        T Visit(BiometricLoginResult.NoAction noAction);
        T Visit(BiometricLoginResult.CouldNotLogin couldNotLogin);
    }
}