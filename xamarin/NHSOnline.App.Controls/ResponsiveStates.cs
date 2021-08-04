using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    internal static class ResponsiveStates
    {
        private const int SmallDisplayResponsiveBreakpoint = 536;
        private const int MediumDisplayResponsiveBreakpoint = 769;

        private enum ResponsiveDisplayStates
        {
            SmallDisplay,
            MediumDisplay,
            LargeDisplay
        }

        internal static void SetVisualStateBreakpoints(VisualElement visualElement)
        {
            VisualStateManager.GoToState(visualElement,
                Device.info.ScaledScreenSize.Width <= MediumDisplayResponsiveBreakpoint
                    ? nameof(ResponsiveDisplayStates.MediumDisplay)
                    : nameof(ResponsiveDisplayStates.LargeDisplay));
        }

        private static ResponsiveDisplayStates GetScreenSize()
        {
            var screenWidth = Device.info.ScaledScreenSize.Width;

            if (screenWidth <= SmallDisplayResponsiveBreakpoint)
            {
                return ResponsiveDisplayStates.SmallDisplay;
            }

            if (screenWidth <= MediumDisplayResponsiveBreakpoint)
            {
                return ResponsiveDisplayStates.MediumDisplay;
            }

            return ResponsiveDisplayStates.LargeDisplay;
        }

        public static bool IsSmallScreen => ResponsiveDisplayStates.SmallDisplay == GetScreenSize();
    }
}