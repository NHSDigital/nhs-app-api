using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    internal static class ResponsiveStates
    {
        private const int ResponsiveBreakpoint = 769;

        private enum ResponsiveDisplayStates
        {
            SmallDisplay,
            LargeDisplay
        }

        internal static void SetVisualStateBreakpoints(VisualElement visualElement)
        {
            VisualStateManager.GoToState(visualElement,
                Device.info.ScaledScreenSize.Width <= ResponsiveBreakpoint
                    ? nameof(ResponsiveDisplayStates.SmallDisplay)
                    : nameof(ResponsiveDisplayStates.LargeDisplay));
        }
    }
}