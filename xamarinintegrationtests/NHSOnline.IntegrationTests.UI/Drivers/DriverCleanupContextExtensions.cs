using System.IO;
using NHSOnline.IntegrationTests.UI.Drivers.Native;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;

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

        internal static void TryAttachNativePageSource(
            this IDriverCleanupContext context,
            IWebDriver driver,
            NativeDriverContext nativeDriverContext)
        {
            context.TryAttach(
                "App page source",
                "PageSourceNative.xml",
                file =>
                {
                    nativeDriverContext.SwitchToNativeContext();
                    File.WriteAllText(file.FullName, driver.PageSource);
                });

            context.TryCleanUp(
                "Web page sources",
                () => nativeDriverContext.ForEachWebView(TryAttachWebContextPageSource));

            void TryAttachWebContextPageSource(IWebContext webContext)
            {
                context.TryAttach(
                    $"Web page source {webContext}",
                    $"PageSourceWeb{webContext}.html",
                    file => File.WriteAllText(file.FullName, driver.ExecuteJavaScript<string>("return document.documentElement.innerHTML;")));
            }
        }

        internal static void UpdateBrowserStackStatusToFailed(
            this IDriverCleanupContext context,
            IHasSessionId driver,
            BrowserStackConfig browserStackConfig)
        {
            context.TryCleanUp("Update BrowserStack status", () =>
            {
                var browserStackClient = new BrowserStackApiClient(browserStackConfig);
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
                var browserStackClient = new BrowserStackApiClient(browserStackConfig);
                browserStackClient.UpdateStatus(driver.SessionId, "Passed", "Integration Test Passed");
            });
        }

        internal static void AddBrowserStackSessionDetailsToLogs(
            this IDriverCleanupContext context,
            IHasSessionId driver,
            BrowserStackConfig browserStackConfig,
            TestLogs testLogs)
        {
            context.TryCleanUp("Add BrowserStack session details to logs", () =>
            {
                var browserStackClient = new BrowserStackApiClient(browserStackConfig);
                var sessionDetails = browserStackClient.GetSessionDetails(driver.SessionId);

                if (sessionDetails != null)
                {
                    testLogs.BrowserStackSessionDetails(sessionDetails.BrowserUrl, sessionDetails.VideoUrl);
                }
            });
        }
    }
}