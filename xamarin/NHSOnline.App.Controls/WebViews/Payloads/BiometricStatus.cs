namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public sealed class BiometricStatus
    {
        private BiometricStatus(string biometricType, bool enabled)
        {
            BiometricType = biometricType;
            Enabled = enabled;
        }

        public string BiometricType { get; }
        public bool Enabled { get; }

        public static BiometricStatus FingerPrintFaceOrIris(bool enabled) => new BiometricStatus("fingerPrintFaceOrIris", enabled);
        public static BiometricStatus TouchId(bool enabled) => new BiometricStatus("touch", enabled);
        public static BiometricStatus FaceId(bool enabled) => new BiometricStatus("face", enabled);
        public static BiometricStatus None() => new BiometricStatus("none", false);
    }
}