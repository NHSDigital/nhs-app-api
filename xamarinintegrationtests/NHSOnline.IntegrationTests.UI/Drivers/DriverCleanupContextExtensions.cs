using System.IO;
using NHSOnline.IntegrationTests.UI.Drivers.Native;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal static class DriverCleanupContextExtensions
    {
        internal static void TryAttachScreenshot(
            this IDriverCleanupContext context,
            IWebDriver driver)
        {
            if (driver is ITakesScreenshot takesScreenShot)
            {
                context.TryAttach(
                    "screen shot",
                    "ScreenShot.png",
                    file => takesScreenShot.GetScreenshot().SaveAsFile(file.FullName));
            }
        }

        internal static void TryAttachWebPageSource(
            this IDriverCleanupContext context,
            IWebDriver driver)
        {
            context.TryAttach(
                "page source",
                "PageSourceWeb.html",
                file => File.WriteAllText(file.FullName, driver.PageSource));
        }

        internal static void TryAttachNativePageSource(
            this IDriverCleanupContext context,
            IWebDriver driver,
            NativeDriverContext nativeDriverContext)
        {
            context.TryAttach(
                "App page source",
                "PageSourceNative.xml",
                file => File.WriteAllText(file.FullName, driver.PageSource));

            context.TryCleanUp(
                "Web page sources",
                () => nativeDriverContext.ForEachWebView(TryAttachWebContextPageSource));

            void TryAttachWebContextPageSource(string webContext)
            {
                context.TryAttach(
                    $"Web page source {webContext}",
                    $"PageSourceWeb{webContext}.html",
                    file => File.WriteAllText(file.FullName, driver.PageSource));
            }
        }

        internal static void UpdateBrowserStackStatusToFailed(
            this IDriverCleanupContext context,
            IHasSessionId driver,
            BrowserStackConfig browserStackConfig)
        {
            context.TryCleanUp("Update BrowserStack status", () =>
            {
                var browserStackClient = new BrowerStackApiClient(browserStackConfig);
                browserStackClient.UpdateStatus(driver.SessionId, "Failed", "Integration Test Failed");
            });
        }

        internal static void UpdateBrowserStackStatusToPassed(
            this IDriverCleanupContext context,
            IHasSessionId driver,
            BrowserStackConfig browserStackConfig)
        {
            context.TryCleanUp("Update BrowserStack status", () =>
            {
                var browserStackClient = new BrowerStackApiClient(browserStackConfig);
                browserStackClient.UpdateStatus(driver.SessionId, "Passed", "Integration Test Passed");
            });
        }
    }
}