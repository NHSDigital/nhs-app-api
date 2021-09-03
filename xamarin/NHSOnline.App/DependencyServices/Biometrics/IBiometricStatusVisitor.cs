namespace NHSOnline.App.DependencyServices.Biometrics
{
    public interface IBiometricStatusVisitor<T>
    {
        T Visit(BiometricStatus.HardwareNotPresent hardwareNotPresent);
        T Visit(BiometricStatus.FingerPrintFaceOrIris fingerPrintFaceOrIris);
        T Visit(BiometricStatus.TouchId touchId);
        T Visit(BiometricStatus.FaceId faceId);
    }
}