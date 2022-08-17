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
            internal HardwarePresent(bool usable, bool registered, bool enrolledAtDeviceLevel)
            {
                Usable = usable;
                Registered = registered;
                EnrolledAtDeviceLevel = enrolledAtDeviceLevel;
            }

            internal bool Usable { get; }
            internal bool Registered { get; }
            internal bool EnrolledAtDeviceLevel { get; }
        }

        internal sealed class FingerPrintFaceOrIris : HardwarePresent
        {
            internal FingerPrintFaceOrIris(bool usable, bool registered, bool enrolledAtDeviceLevel) : base(usable, registered, enrolledAtDeviceLevel)
            {
            }

            public override T Accept<T>(IBiometricStatusResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class TouchId : HardwarePresent
        {
            internal TouchId(bool usable, bool registered, bool enrolledAtDeviceLevel) : base(usable, registered, enrolledAtDeviceLevel)
            {
            }

            public override T Accept<T>(IBiometricStatusResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class FaceId : HardwarePresent
        {
            internal FaceId(bool usable, bool registered, bool enrolledAtDeviceLevel) : base(usable, registered, enrolledAtDeviceLevel)
            {
            }

            public override T Accept<T>(IBiometricStatusResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class LegacySensorNotValid : BiometricStatusResult
        {
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

            public BiometricStatusResult Visit(BiometricStatus.FingerPrintFaceOrIris fingerPrintFaceOrIris)
            {
                var usable = fingerPrintFaceOrIris.State == BiometricHardwareState.Usable;
                var registered = IsRegistered(fingerPrintFaceOrIris.RegistrationStatus);
                var enrolled = fingerPrintFaceOrIris.Enrolled;
                return new FingerPrintFaceOrIris(usable, registered, enrolled);
            }

            public BiometricStatusResult Visit(BiometricStatus.TouchId touchId)
            {
                var usable = touchId.State == BiometricHardwareState.Usable;
                var registered = IsRegistered(touchId.RegistrationStatus);
                var enrolled = touchId.Enrolled;
                return new TouchId(usable, registered, enrolled);
            }

            public BiometricStatusResult Visit(BiometricStatus.FaceId faceId)
            {
                var usable = faceId.State == BiometricHardwareState.Usable;
                var registered = IsRegistered(faceId.RegistrationStatus);
                var enrolled = faceId.Enrolled;
                return new FaceId(usable, registered, enrolled);
            }

            public BiometricStatusResult Visit(BiometricStatus.LegacySensorNotValid legacySensorNotValid)
            {
                return new LegacySensorNotValid();
            }

            private bool IsRegistered(BiometricRegistrationStatus registrationStatus)
            {
                return registrationStatus switch
                {
                    BiometricRegistrationStatus.Registered => _hasKeyId,
                    BiometricRegistrationStatus.Invalidated => false,
                    _ => false
                };
            }
        }
    }
}