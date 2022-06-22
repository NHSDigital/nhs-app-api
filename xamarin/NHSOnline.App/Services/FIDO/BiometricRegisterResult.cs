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
        public static BiometricRegisterResult UserCancelled() => new BiometricRegisterResult(BiometricOutcome.UserCancelled);
        public static BiometricRegisterResult SystemCancelled() => new BiometricRegisterResult(BiometricOutcome.SystemCancelled);
        public static BiometricRegisterResult NotInteractive() => new BiometricRegisterResult(BiometricOutcome.NotInteractive);

        public BiometricOutcome Outcome { get; }
        public BiometricErrorCode? ErrorCode { get; }
    }
}