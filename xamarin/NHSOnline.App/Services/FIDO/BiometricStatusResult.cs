using NHSOnline.App.DependencyServices.Biometrics;

namespace NHSOnline.App.Services.FIDO
{
    internal abstract class BiometricStatusResult
    {
        private BiometricStatusResult()
        {
        }

        public abstract T Accept<T>(IBiometricStatusResultVisitor<T> visitor);

        internal static BiometricStatusResult DeriveFrom(BiometricStatus status, bool hasKeyId)
            => status.Accept(new BiometricStatusVisitor(hasKeyId));

        internal sealed class HardwareNotPresent : BiometricStatusResult
        {
            public override T Accept<T>(IBiometricStatusResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal abstract class HardwarePresent : BiometricStatusResult
        {
            internal HardwarePresent(bool usable, bool registered)
            {
                Usable = usable;
                Registered = registered;
            }

            internal bool Usable { get; }
            internal bool Registered { get; }
        }

        internal sealed class FingerPrint : HardwarePresent
        {
            internal FingerPrint(bool usable, bool registered) : base(usable, registered) { }
            public override T Accept<T>(IBiometricStatusResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class TouchId : HardwarePresent
        {
            internal TouchId(bool usable, bool registered) : base(usable, registered) { }
            public override T Accept<T>(IBiometricStatusResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class FaceId : HardwarePresent
        {
            internal FaceId(bool usable, bool registered) : base(usable, registered) { }
            public override T Accept<T>(IBiometricStatusResultVisitor<T> visitor) => visitor.Visit(this);
        }


        private sealed class BiometricStatusVisitor : IBiometricStatusVisitor<BiometricStatusResult>
        {
            private readonly bool _hasKeyId;

            public BiometricStatusVisitor(bool hasKeyId)
            {
                _hasKeyId = hasKeyId;
            }

            public BiometricStatusResult Visit(BiometricStatus.HardwareNotPresent hardwareNotPresent)
            {
                return new HardwareNotPresent();
            }

            public BiometricStatusResult Visit(BiometricStatus.FingerPrint fingerPrint)
            {
                var usable = fingerPrint.State == BiometricHardwareState.Usable;
                var registered = _hasKeyId && fingerPrint.RegistrationStatus == BiometricRegistrationStatus.Registered;
                return new FingerPrint(usable, registered);
            }

            public BiometricStatusResult Visit(BiometricStatus.TouchId touchId)
            {
                var usable = touchId.State == BiometricHardwareState.Usable;
                var registered = _hasKeyId && touchId.RegistrationStatus == BiometricRegistrationStatus.Registered;
                return new TouchId(usable, registered);
            }

            public BiometricStatusResult Visit(BiometricStatus.FaceId faceId)
            {
                var usable = faceId.State == BiometricHardwareState.Usable;
                var registered = _hasKeyId && faceId.RegistrationStatus == BiometricRegistrationStatus.Registered;
                return new FaceId(usable, registered);
            }
        }
    }
}