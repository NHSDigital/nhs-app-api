namespace NHSOnline.App.Services.FIDO
{
    internal interface IBiometricStatusResultVisitor<T>
    {
        T Visit(BiometricStatusResult.HardwareNotPresent hardwareNotPresent);
        T Visit(BiometricStatusResult.FingerPrintFaceOrIris fingerPrintFaceOrIris);
        T Visit(BiometricStatusResult.TouchId touchId);
        T Visit(BiometricStatusResult.FaceId faceId);
    }
}