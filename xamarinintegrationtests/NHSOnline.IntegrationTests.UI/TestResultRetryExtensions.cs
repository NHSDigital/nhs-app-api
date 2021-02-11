using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    internal static class TestResultRetryExtensions
    {
        internal const string DeviceTimeSkewMessage = "device time should be close to the current time";

        // 308536-Invalid Service com.apple.webinspector
        private static readonly Regex InvalidServiceWebInspectorMessage = new Regex(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: Unexpected data: {""Error"":""InvalidService"",""Request"":""StartService"",""Service"":""com.apple.webinspector""}",
            RegexOptions.Compiled);

        // 389382-Got response with status 200: disconnected: unable to connect to renderer
        private static readonly Regex UnableToConnectToRenderer = new Regex(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: disconnected: unable to connect to renderer",
            RegexOptions.Compiled);

        // 388660-Error executing adbExec - adb: error: listener 'tcp:9222' not found
        private static readonly Regex AdbErrorListenerNotFound = new Regex(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: Error executing adbExec\. .* Stderr: 'adb: error: listener 'tcp:9222' not found'",
            RegexOptions.Compiled);

        // 390309-Device time in the future
        private static readonly Regex DeviceTimeSkew = new Regex(
            Regex.Escape(DeviceTimeSkewMessage),
            RegexOptions.Compiled);

        // 390078-Incorrect Chrome Version
        private static readonly Regex IncorrectChromeVersion = new Regex(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: A new session could not be created\. Details: session not created: This version of ChromeDriver only supports Chrome version",
            RegexOptions.Compiled);

        // NHSO-13172-Javascript failed to load
        private static readonly Regex JavascriptLoadFailure = new Regex(
            @"Assert\.IsTrue failed\. window\.nhsAppPageLoadComplete was not found to be true",
            RegexOptions.Compiled);

        private static readonly List<Regex> RetryExceptionMessageRegexes = new List<Regex>
        {
            InvalidServiceWebInspectorMessage,
            UnableToConnectToRenderer,
            AdbErrorListenerNotFound,
            DeviceTimeSkew,
            IncorrectChromeVersion,
            JavascriptLoadFailure
        };

        internal static bool ShouldRetry(this TestResult result, TestLogs logs)
        {
            var exception = result.TestFailureException;
            if (result.Outcome == UnitTestOutcome.Passed || exception == null)
            {
                return false;
            }

            var exceptionType = exception.GetType();
            logs.Info("TestFailureException type={0}; Message={1}", exceptionType.FullName ?? exceptionType.Name, exception.Message);

            return RetryExceptionMessageRegexes.Any(IsMatch);

            bool IsMatch(Regex regex) => regex.IsMatch(exception.Message);
        }
    }
}