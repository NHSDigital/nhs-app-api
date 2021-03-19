namespace NHSOnline.App.Services.FIDO
{
    internal sealed class BiometricDeleteRegistrationResult
    {
        public BiometricOutcome Outcome { get; }
        public BiometricErrorCode? ErrorCode { get; }
    }
}