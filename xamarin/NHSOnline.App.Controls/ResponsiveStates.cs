using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    public static class ResponsiveStates
    {
        private const int SmallDisplayResponsiveBreakpoint = 325;
        private const int MediumDisplayResponsiveBreakpoint = 769;

        private enum ResponsiveDisplayStates
        {
            SmallDisplay,
            MediumDisplay,
            LargeDisplay
        }

        public static void SetVisualStateBreakpoints(VisualElement visualElement)
        {
            VisualStateManager.GoToState(visualElement, GetScreenSize().ToString());
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
    }
}