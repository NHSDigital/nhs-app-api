using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidSlimCloseNavigation
    {
        private readonly IAndroidDriverWrapper _driver;

        internal AndroidSlimCloseNavigation(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidIcon CloseIcon => AndroidIcon.WithName(_driver, "NHS App close icon");

        public void Close() => CloseIcon.Click();

        internal AndroidKeyboardNavigation KeyboardNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(_driver, CloseIcon);
    }
}