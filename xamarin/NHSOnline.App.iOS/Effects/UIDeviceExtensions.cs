using UIKit;

namespace NHSOnline.App.iOS.Effects
{
    internal static class UIDeviceExtensions
    {
        internal static bool SupportsSafeAreaInsets(this UIDevice device)
        {
            return device.CheckSystemVersion(11, 0);
        }
    }
}