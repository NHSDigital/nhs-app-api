namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public sealed class BiometricSpec
    {
        private BiometricSpec(string biometricTypeReference, bool enabled)
        {
            BiometricTypeReference = biometricTypeReference;
            Enabled = enabled;
        }

        public string BiometricTypeReference { get; }
        public bool Enabled { get; }

        public static BiometricSpec FingerPrint(bool enabled) => new BiometricSpec("fingerPrint", enabled);
        public static BiometricSpec TouchId(bool enabled) => new BiometricSpec("touch", enabled);
        public static BiometricSpec FaceId(bool enabled) => new BiometricSpec("face", enabled);
    }
}