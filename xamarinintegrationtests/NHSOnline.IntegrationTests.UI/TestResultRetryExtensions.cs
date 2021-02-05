using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI
{
    internal static class TestResultRetryExtensions
    {
        private const string InvalidServiceWebInspectorMessage = "OpenQA.Selenium.WebDriverException: Appium error: An unknown server-side error occurred while processing the command. Original error: Unexpected data: {\"Error\":\"InvalidService\",\"Request\":\"StartService\",\"Service\":\"com.apple.webinspector\"})";

        internal static bool ShouldRetry(this TestResult result)
        {
            if (result.Outcome == UnitTestOutcome.Passed)
            {
                return false;
            }

            return result.TestFailureException switch
            {
                WebDriverException webDriverException when webDriverException.ShouldRetry() => true,
                _ => false
            };
        }

        private static bool ShouldRetry(this WebDriverException webDriverException)
        {
            return webDriverException.Message.Contains(InvalidServiceWebInspectorMessage, StringComparison.OrdinalIgnoreCase);
        }
    }
}