namespace NHSOnline.App.Services.FIDO
{
    internal sealed class BiometricErrorCode
    {
        private readonly string _errorCode;

        private BiometricErrorCode(string errorCode) => _errorCode = errorCode;

        internal static BiometricErrorCode CannotFindBiometrics { get; } = new BiometricErrorCode("10004");

        internal static BiometricErrorCode CannotChangeBiometrics { get; } = new BiometricErrorCode("10005");

        internal static BiometricErrorCode CannotUseBiometrics { get; } = new BiometricErrorCode("10007");

        public override string ToString() => _errorCode;
    }
}