using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay
{
    public class AndroidBrowserOverlay
    {
        private readonly IAndroidDriverWrapper _driver;
        private readonly string _text;

        private AndroidBrowserOverlay(IAndroidDriverWrapper driver, string text = "")
        {
            _driver = driver;
            _text = text;
        }

        private AndroidBrowserOverlayContents Title => AndroidBrowserOverlayContents.WithText(_driver, _text);

        private AndroidBrowserOverlayContents ErrorCodeText(string errorCodeSubstring) =>
            AndroidBrowserOverlayContents.WithText(_driver, errorCodeSubstring);

        private AndroidLabel NoInternetText => AndroidLabel.WhichMatches(_driver, "(?i)No internet");

        private AndroidImageButton CloseTabButton => AndroidImageButton.WithDescription(_driver, "Close tab");

        private AndroidButton AcceptNhsCookiesButton => AndroidButton.WithText(_driver, "I'm OK with analytics cookies");

        public void AssertNoInternet() => NoInternetText.AssertVisible();

        public void ReturnToApp() => CloseTabButton.Click();

        public static AndroidBrowserOverlay AssertInBrowserOverlay(IAndroidDriverWrapper driver, string title = "")
        {
            var androidBrowserOverlay = new AndroidBrowserOverlay(driver, title);
            androidBrowserOverlay.CloseTabButton.AssertVisible();
            return androidBrowserOverlay;
        }

        public void AssertOnPage()
        {
            if (AcceptNhsCookiesButton.IsVisible())
            {
                AcceptNhsCookiesButton.Click();
            }

            Title.AssertVisible();
        }

        internal void AssertErrorCode(string errorCode) =>
            ErrorCodeText($"errorcode: {errorCode}").AssertSubStringVisible();
    }
}
