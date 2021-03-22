namespace NHSOnline.App.Services.FIDO
{
    internal interface IBiometricStatusResultVisitor<T>
    {
        T Visit(BiometricStatusResult.HardwareNotPresent hardwareNotPresent);
        T Visit(BiometricStatusResult.FingerPrint fingerPrint);
        T Visit(BiometricStatusResult.TouchId touchId);
        T Visit(BiometricStatusResult.FaceId faceId);
    }
}