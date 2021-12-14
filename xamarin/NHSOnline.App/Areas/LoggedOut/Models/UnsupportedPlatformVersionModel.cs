namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class UnsupportedPlatformVersionModel
    {
        public UnsupportedPlatformVersionModel(string minimumPlatformVersion)
        {
            MinimumPlatformVersion = minimumPlatformVersion;
        }

        internal string MinimumPlatformVersion { get; }
    }
}