using System.IO;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    public class FlipbookGeneration
    {
        private readonly string FlipBookPath = "../../../../flipbook/";

        public FlipbookGeneration(string testName)
        {
            FlipBookPath = $"{FlipBookPath}{testName}";
            Directory.CreateDirectory($"{FlipBookPath}/screenshots");
        }

        public void Screenshot(IBrowserStackDriver driver, string screenshotName)
        {
            var path = $"{FlipBookPath}/screenshots";

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("No screenshots directory found. Has it been created?");
            }

            driver.GetScreenshot().SaveAsFile($"{path}/{screenshotName}.png");
        }

        public void WriteTestDetails(string appVersion, string device, string make, string osVersion)
        {
            var testDetails = new FileInfo($"{FlipBookPath}/testDetails.txt");

            using StreamWriter sw = testDetails.CreateText();
            sw.WriteLine(appVersion);
            sw.WriteLine(device);
            sw.WriteLine(make);
            sw.WriteLine(osVersion);
        }
    }
}