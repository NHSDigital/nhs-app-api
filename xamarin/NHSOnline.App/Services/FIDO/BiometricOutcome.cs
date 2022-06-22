namespace NHSOnline.App.Services.FIDO
{
    internal enum BiometricOutcome
    {
        Success,
        Failed,
        UserCancelled,
        SystemCancelled,
        NotInteractive
    }
}