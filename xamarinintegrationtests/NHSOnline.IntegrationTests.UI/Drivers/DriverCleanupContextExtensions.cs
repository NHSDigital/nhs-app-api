using System.IO;
using OpenQA.Selenium;

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
                context.TryAttach("screen shot", () =>
                {
                    var screenShot = takesScreenShot.GetScreenshot();
                    var fileName = Path.Join(Path.GetTempPath(), $"{context.TestName}.png");
                    screenShot.SaveAsFile(fileName);
                    return fileName;
                });
            }
        }

        internal static void TryAttachWebPageSource(
            this IDriverCleanupContext context,
            IWebDriver driver)
        {
            context.TryAttach("page source", () =>
            {
                var pageSource = driver.PageSource;
                var fileName = Path.Join(Path.GetTempPath(), $"{context.TestName}.html");
                File.WriteAllText(fileName, pageSource);
                return fileName;
            });
        }

        internal static void TryAttachNativePageSource(
            this IDriverCleanupContext context,
            IWebDriver driver,
            NativeDriverContext nativeDriverContext)
        {
            context.TryAttach("App page source", () =>
            {
                var pageSource = driver.PageSource;
                var fileName = Path.Join(Path.GetTempPath(), $"{context.TestName}.xml");
                File.WriteAllText(fileName, pageSource);
                return fileName;
            });

            context.TryCleanUp(
                "Web page sources",
                () => nativeDriverContext.ForEachWebView(TryAttachWebContextPageSource));

            void TryAttachWebContextPageSource(string webContext)
            {
                context.TryAttach($"Web page source {webContext}", () =>
                {
                    var pageSource = driver.PageSource;
                    var fileName = Path.Join(Path.GetTempPath(), $"{context.TestName} {webContext}.html");
                    File.WriteAllText(fileName, pageSource);
                    return fileName;
                });
            }
        }
    }
}