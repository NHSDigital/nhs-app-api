using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidAppTab
    {
        private readonly IAndroidDriverWrapper _driver;
        private readonly string _headerText;

        private AndroidAppTab(IAndroidDriverWrapper driver, string headerText = "")
        {
            _driver = driver;
            _headerText = headerText;
        }

        private AndroidLabel BrowserChoicePromptChromeOption => new AndroidLabel(_driver, "Chrome");
        private AndroidButton BrowserChoicePromptJustOnceOption => new AndroidButton(_driver, "JUST ONCE");
        private AndroidView AppTabHeader => new AndroidView(_driver, _headerText);
        private AndroidImageButton AndroidAppTabClose => new AndroidImageButton(_driver, "Close tab");

        internal static void AssertFirstAppTabServiceByHeader(IAndroidDriverWrapper driver, string headerText)
        {
            var page = new AndroidAppTab(driver, headerText);
            page.BrowserChoicePromptChromeOption.Click();
            page.BrowserChoicePromptJustOnceOption.Click();
            page.AppTabHeader.AssertVisible();
            page.AndroidAppTabClose.Click();
        }

        internal static void AssertSubsequentAppTabServiceByHeader(IAndroidDriverWrapper driver, string headerText)
        {
            var page = new AndroidAppTab(driver, headerText);
            page.BrowserChoicePromptJustOnceOption.Click();
            page.AppTabHeader.AssertVisible();
            page.AndroidAppTabClose.Click();
        }
    }
}