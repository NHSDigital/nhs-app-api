namespace NHSOnline.App.DependencyServices.Biometrics
{
    public abstract class BiometricSpec
    {
        private BiometricSpec(bool enabled)
        {
            Enabled = enabled;
        }

        public static BiometricSpec FingerPrint(bool enabled) => new FingerPrintSpec(enabled);
        public static BiometricSpec Touch(bool enabled) => new TouchSpec(enabled);
        public static BiometricSpec Face(bool enabled) => new FaceSpec(enabled);

        public abstract BiometricTypeReference BiometricTypeReference { get; }
        public bool Enabled { get; }

        private sealed class FingerPrintSpec : BiometricSpec
        {
            public FingerPrintSpec(bool enabled) : base(enabled)
            {
            }

            public override BiometricTypeReference BiometricTypeReference => BiometricTypeReference.FingerPrint;
        }

        private sealed class FaceSpec : BiometricSpec
        {
            public FaceSpec(bool enabled) : base(enabled)
            {
            }

            public override BiometricTypeReference BiometricTypeReference => BiometricTypeReference.Face;
        }

        private sealed class TouchSpec : BiometricSpec
        {
            public TouchSpec(bool enabled) : base(enabled)
            {
            }

            public override BiometricTypeReference BiometricTypeReference => BiometricTypeReference.Touch;
        }
    }
}