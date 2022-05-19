using System;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSNativeBackActionPage
    {
        private IOSNativeBackActionPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new NativeBackActionPageContent(driver.Web.WebIntegrationWebView());
        }

        private IOSFullNavigation Navigation { get; }

        public NativeBackActionPageContent PageContent { get; }

        public static IOSNativeBackActionPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSNativeBackActionPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSNativeBackActionPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }


        public IOSNativeBackActionPage EnterNativeBackAction(IIOSDriverWrapper driver, String function)
        {
            var page = new IOSNativeBackActionPage(driver);
            page.PageContent.EnterNativeBackAction(function);
            return page;
        }

        public IOSNativeBackActionPage ClickSimulateBackButton(IIOSDriverWrapper driver)
        {
            var page = new IOSNativeBackActionPage(driver);
            page.PageContent.ClickSimulateBackButton();
            return page;
        }

        public IOSNativeBackActionPage ClickSetBackActionButton(IIOSDriverWrapper driver)
        {
            var page = new IOSNativeBackActionPage(driver);
            page.PageContent.ClickSetBackActionButton();
            return page;
        }
    }
}