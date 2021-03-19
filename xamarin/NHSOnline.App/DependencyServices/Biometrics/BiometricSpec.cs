namespace NHSOnline.App.DependencyServices.Biometrics
{
    public abstract class BiometricStatus
    {
        private BiometricStatus(bool enabled)
        {
            Enabled = enabled;
        }

        public static BiometricStatus FingerPrint(bool enabled) => new FingerPrintStatus(enabled);
        public static BiometricStatus Touch(bool enabled) => new TouchStatus(enabled);
        public static BiometricStatus Face(bool enabled) => new FaceStatus(enabled);

        public abstract string BiometricTypeReference { get; }
        public bool Enabled { get; }

        private sealed class FingerPrintStatus : BiometricStatus
        {
            public FingerPrintStatus(bool enabled) : base(enabled)
            {
            }

            public override string BiometricTypeReference => "fingerPrint";
        }

        private sealed class FaceStatus : BiometricStatus
        {
            public FaceStatus(bool enabled) : base(enabled)
            {
            }

            public override string BiometricTypeReference => "face";
        }

        private sealed class TouchStatus : BiometricStatus
        {
            public TouchStatus(bool enabled) : base(enabled)
            {
            }

            public override string BiometricTypeReference => "touch";
        }
    }
}