using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay
{
    public class IOSBrowserOverlay
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly string _title;

        private IOSBrowserOverlay(IIOSDriverWrapper driver, string title = "")
        {
            _driver = driver;
            _title = title;
        }

        private IOSButton DoneButton => IOSButton.WithText(_driver, "Done");

        private IOSLabel TitleText => IOSLabel.WithText(_driver, _title);

        private WebText ErrorCodeText(IWebInteractor webInteractor, string errorCode) => WebText.WithText(webInteractor, errorCode);

        private IOSLabel NoInternetText => IOSLabel.WithText(_driver, "Safari cannot open the page because your iPhone is not connected to the internet.");

        private IOSButton AcceptAnalyticsCookiesButton => IOSButton.WithText(_driver, "I'm OK with analytics cookies");

        public void AssertNoInternet() => NoInternetText.AssertVisible();

        public void ReturnToApp() => DoneButton.Click();

        public static IOSBrowserOverlay AssertInBrowserOverlay(IIOSDriverWrapper driver, string title = "")
        {
            var browserOverlay = new IOSBrowserOverlay(driver, title);
            browserOverlay.DoneButton.AssertVisible();
            return browserOverlay;
        }

        public void AssertOnPage()
        {
            if (AcceptAnalyticsCookiesButton.IsVisible())
            {
                AcceptAnalyticsCookiesButton.Click();
            }

            TitleText.AssertVisible();
        }

        public void AssertErrorCode(string errorCode) =>
            ErrorCodeText(_driver.Web.BrowserOverlayWebView(), $"errorcode: {errorCode}").AssertContainsVisible();
    }
}
