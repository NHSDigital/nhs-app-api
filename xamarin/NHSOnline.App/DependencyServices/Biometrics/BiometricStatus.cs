namespace NHSOnline.App.DependencyServices.Biometrics
{
    public abstract class BiometricStatus
    {
        public abstract T Accept<T>(IBiometricStatusVisitor<T> visitor);

        public sealed class HardwareNotPresent : BiometricStatus
        {
            public override T Accept<T>(IBiometricStatusVisitor<T> visitor) => visitor.Visit(this);
        }

        public abstract class HardwarePresent: BiometricStatus
        {
            internal HardwarePresent(BiometricHardwareState state, BiometricRegistrationStatus registrationStatus)
            {
                State = state;
                RegistrationStatus = registrationStatus;
            }

            public BiometricHardwareState State { get; }
            public BiometricRegistrationStatus RegistrationStatus { get; }
        }

        public sealed class FingerPrintFaceOrIris : HardwarePresent
        {
            public FingerPrintFaceOrIris(BiometricHardwareState state, BiometricRegistrationStatus registrationStatus)
                : base(state, registrationStatus)
            {
            }

            public override T Accept<T>(IBiometricStatusVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class FaceId : HardwarePresent
        {
            public FaceId(BiometricHardwareState state, BiometricRegistrationStatus registrationStatus)
                : base(state, registrationStatus)
            {
            }

            public override T Accept<T>(IBiometricStatusVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class TouchId : HardwarePresent
        {
            public TouchId(BiometricHardwareState state, BiometricRegistrationStatus registrationStatus)
                : base(state, registrationStatus)
            {
            }

            public override T Accept<T>(IBiometricStatusVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}