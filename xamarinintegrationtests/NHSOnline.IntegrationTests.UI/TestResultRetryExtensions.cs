using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    internal static class TestResultRetryExtensions
    {
        internal const string DeviceTimeSkewMessage = "device time should be close to the current time";

        // 308536-Invalid Service com.apple.webinspector
        private static readonly Regex InvalidServiceWebInspectorMessage = new(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: Unexpected data: {""Error"":""InvalidService"",""Request"":""StartService"",""Service"":""com.apple.webinspector""}",
            RegexOptions.Compiled);

        // 389382-Got response with status 200: disconnected: unable to connect to renderer
        private static readonly Regex UnableToConnectToRenderer = new(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: disconnected: unable to connect to renderer",
            RegexOptions.Compiled);

        // 388660-Error executing adbExec - adb: error: listener 'tcp:9222' not found
        private static readonly Regex AdbErrorListenerNotFound = new(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: Error executing adbExec\. .* Stderr: 'adb: error: listener 'tcp:9222' not found'",
            RegexOptions.Compiled);

        // 390309-Device time in the future
        private static readonly Regex DeviceTimeSkew = new(
            Regex.Escape(DeviceTimeSkewMessage),
            RegexOptions.Compiled);

        // 390078-Incorrect Chrome Version
        private static readonly Regex IncorrectChromeVersion = new(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: A new session could not be created\. Details: session not created: This version of ChromeDriver only supports Chrome version",
            RegexOptions.Compiled);

        // NHSO-13172-Javascript failed to load
        private static readonly Regex JavascriptLoadFailure = new(
            @"Assert\.IsTrue failed\. window\.nhsAppPageLoadComplete was not found to be true",
            RegexOptions.Compiled);

        private static readonly List<(Regex pattern, RetryStatus result)> RetryExceptionMessageRegexes = new()
        {
            (InvalidServiceWebInspectorMessage, RetryStatus.Retry(nameof(InvalidServiceWebInspectorMessage))),
            (UnableToConnectToRenderer, RetryStatus.Retry(nameof(UnableToConnectToRenderer))),
            (AdbErrorListenerNotFound, RetryStatus.Retry(nameof(AdbErrorListenerNotFound))),
            (DeviceTimeSkew, RetryStatus.Retry(nameof(DeviceTimeSkew))),
            (IncorrectChromeVersion, RetryStatus.Retry(nameof(IncorrectChromeVersion))),
            (JavascriptLoadFailure, RetryStatus.Retry(nameof(JavascriptLoadFailure)))
        };

        internal static RetryStatus ShouldRetry(this TestResult result, TestLogs logs)
        {
            var exception = result.TestFailureException;
            if (result.Outcome == UnitTestOutcome.Passed || exception == null)
            {
                return RetryStatus.NoRetry;
            }

            var exceptionType = exception.GetType();
            logs.Info("TestFailureException type={0}; Message={1}", exceptionType.FullName ?? exceptionType.Name, exception.Message);

            foreach (var (pattern, retryResult) in RetryExceptionMessageRegexes)
            {
                if (pattern.IsMatch(exception.Message))
                {
                    return retryResult;
                }
            }

            return RetryStatus.NoRetry;
        }
    }
}