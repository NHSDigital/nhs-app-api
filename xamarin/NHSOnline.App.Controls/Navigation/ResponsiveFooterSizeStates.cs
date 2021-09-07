using Xamarin.Forms;

namespace NHSOnline.App.Controls.Navigation
{
    internal static class ResponsiveFooterSizeStates
    {
        private const int WideDisplayResponsiveBreakpoint = 650;

        private enum ResponsiveFooterDisplaySizeStates
        {
            DefaultDisplay,
            WideDisplay,
        }

        internal static void SetFooterWidthVisualState(VisualElement visualElement)
        {
            var state = Device.info.ScaledScreenSize.Width <= WideDisplayResponsiveBreakpoint
                ? nameof(ResponsiveFooterDisplaySizeStates.DefaultDisplay)
                : nameof(ResponsiveFooterDisplaySizeStates.WideDisplay);
            VisualStateManager.GoToState(visualElement, state);
        }
    }
}