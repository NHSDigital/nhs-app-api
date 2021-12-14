using UIKit;

namespace NHSOnline.App.iOS
{
    public static class Compatibility
    {
        public static bool MinimumRequiredVersion(int major, int minor) =>
            UIDevice.CurrentDevice.CheckSystemVersion(major, minor);
    }
}