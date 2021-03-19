namespace NHSOnline.App.Services.FIDO
{
    internal sealed class BiometricRegisterResult
    {
        private BiometricRegisterResult(BiometricOutcome outcome, BiometricErrorCode? errorCode = null)
        {
            Outcome = outcome;
            ErrorCode = errorCode;
        }

        public static BiometricRegisterResult Success() => new BiometricRegisterResult(BiometricOutcome.Success);
        public static BiometricRegisterResult Failed(BiometricErrorCode errorCode) => new BiometricRegisterResult(BiometricOutcome.Failed, errorCode);
        public static BiometricRegisterResult Cancelled() => new BiometricRegisterResult(BiometricOutcome.Cancelled);

        public BiometricOutcome Outcome { get; }
        public BiometricErrorCode? ErrorCode { get; }
    }
}