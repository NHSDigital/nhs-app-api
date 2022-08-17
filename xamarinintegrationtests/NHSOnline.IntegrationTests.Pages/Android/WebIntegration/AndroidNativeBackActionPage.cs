using System;
using AngleSharp;
using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidNativeBackActionPage
    {
        private AndroidNativeBackActionPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new NativeBackActionPageContent(driver.Web.WebIntegrationWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        public NativeBackActionPageContent PageContent { get; }

        public static AndroidNativeBackActionPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNativeBackActionPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidNativeBackActionPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }


        public AndroidNativeBackActionPage EnterNativeBackAction(IAndroidDriverWrapper driver, String function)
        {
            var page = new AndroidNativeBackActionPage(driver);
            page.PageContent.EnterNativeBackAction(function);
            return page;
        }

        public AndroidNativeBackActionPage ClickSimulateBackButton(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNativeBackActionPage(driver);
            page.PageContent.ClickSimulateBackButton();
            return page;
        }

        public AndroidNativeBackActionPage PressBackButton(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNativeBackActionPage(driver);
            driver.PressBackButton();
            return page;
        }

        public AndroidNativeBackActionPage ClickSetBackActionButton(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNativeBackActionPage(driver);
            page.PageContent.ClickSetBackActionButton();
            return page;
        }
    }
}