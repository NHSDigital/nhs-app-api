using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    public class FlipbookGeneration
    {
        private string FlipBookBasePath { get; }
        private string FlipBookTestPath { get; }

        private static readonly List<FlipbookTestDetails> FlipbookTestDetailsList = new();

        public FlipbookGeneration(string flipBookPath, string testName)
        {
            FlipBookBasePath = $"{flipBookPath}";
            FlipBookTestPath = $"{FlipBookBasePath}{testName}";
        }

        public void Screenshot(IBrowserStackDriver driver, string screenshotName)
        {
            var path = $"{FlipBookTestPath}/screenshots";
            var screenshotNameWithTime = $"{DateTimeOffset.Now.ToUnixTimeSeconds()}_{screenshotName}";

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("No screenshots directory found. Has it been created?");
            }

            driver.GetScreenshot().SaveAsFile($"{path}/{screenshotNameWithTime}.png");
        }

        public void WriteTestDetails(FlipbookTestDetails details)
        {
            FlipbookTestDetailsList.Add(details);
            File.WriteAllText($"{FlipBookBasePath}testDetails.json", JsonConvert.SerializeObject(FlipbookTestDetailsList));
        }
    }
}