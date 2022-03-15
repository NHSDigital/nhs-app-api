using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    internal static class TestResultRetryExtensions
    {
        internal const string FailedToCreateDriverMessage = "Failed to create driver";
        internal const string DeviceTimeSkewMessage = "device time should be close to the current time";
        internal const string AppNotRunningMessage = "application should have been automatically started on test device";
        internal const string FailedToUpdateNetworkStateMessage = "Failed to update network state";
        internal const string UnableToVerifyAppVersion = "Failed to verify app version due to device connection";
        internal const string BrowserStackGoogleServicesFailure = "Failed to establish google services connection";

        // 308536-Invalid Service com.apple.webinspector
        private static readonly Regex InvalidServiceWebInspectorMessage = new(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: Unexpected data: {""Error"":""InvalidService"",""Request"":""StartService"",""Service"":""com.apple.webinspector""}",
            RegexOptions.Compiled);

        // 388660-Error executing adbExec - adb: error: listener 'tcp:9222' not found
        private static readonly Regex AdbErrorListenerNotFound = new(
            @"Error executing adbExec",
            RegexOptions.Compiled);

        // Failed to create driver
        private static readonly Regex FailedToCreateDriver = new(
            Regex.Escape(FailedToCreateDriverMessage),
            RegexOptions.Compiled);

        /// Network state did not update in time on the test device
        private static readonly Regex FailedToUpdateNetworkState = new(
            Regex.Escape(FailedToUpdateNetworkStateMessage),
            RegexOptions.Compiled);

        // 390309-Device time in the future
        private static readonly Regex DeviceTimeSkew = new(
            Regex.Escape(DeviceTimeSkewMessage),
            RegexOptions.Compiled);

        // 401785-Failed to start remote service "com.apple.instruments.remoteserver.DVTSecureSocketProxy" on device.
        private static readonly Regex AppNotRunning = new(
            Regex.Escape(AppNotRunningMessage),
            RegexOptions.Compiled);

        // 390078-Incorrect Chrome Version
        private static readonly Regex IncorrectChromeVersion = new(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: A new session could not be created\. Details: session not created: This version of ChromeDriver only supports Chrome version",
            RegexOptions.Compiled);

        // NHSO-13172-Javascript failed to load
        private static readonly Regex JavascriptLoadFailure = new(
            @"Assert\.IsTrue failed\. window\.nhsAppPageLoadComplete was not found to be true",
            RegexOptions.Compiled);

        // 308524-Encountered internal error running command: Error: Did not get any response for atom execution after 120047ms
        private static readonly Regex AtomExecutionTimeout = new(
            @"Did not get any response for atom execution",
            RegexOptions.Compiled);

        // Some BrowserStack iOS devices do not display the camera permissions dialog
        private static readonly Regex UnableToAccessCamera = new(
            @"Please go to Settings to provide application access to camera\.",
            RegexOptions.Compiled);

        // NHSO-13894 - Appium/BrowserStack nginx returned 502
        private static readonly Regex NginxBadGateway = new(
            @"<html>\\n<head><title>502 Bad Gateway</title></head>\\n<body>\\n<center><h1>502 Bad Gateway</h1></center>\\n<hr><center>nginx</center>\\n</body>\\n</html>",
            RegexOptions.Compiled);

        // This is a temporary retry put in place for the manage notifications screen which on android is having firebase issues
        private static readonly Regex FirebaseAuthorisationFailureWontRetry = new(
            @"Unable to locate element.*Manage notifications|No IWebElement found matching.*Allow notificationsI accept the NHS App sending notifications on this device",
            RegexOptions.Compiled);

        // Appium could not translate web coordinates issues
        private static readonly Regex AppiumUnableToTranslateWebCoordinates = new(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: No webviews found\. Unable to translate web coordinates for native web tap.*",
            RegexOptions.Compiled);

        // Appium could not proxy issue
        private static readonly Regex AppiumProxyIssue = new(
            @"Appium error: An unknown server-side error occurred while processing the command\. Original error: Could not proxy.*",
            RegexOptions.Compiled);

        // Appium could not proxy issue POST element
        private static readonly Regex AppiumProxyIssuePostElement = new(
            @"Appium error: An unknown server-side error occurred while processing the command. Original error: 'POST /element'.*",
            RegexOptions.Compiled);

        // 419002-Intermittent network access issue on iOS device
        private static readonly Regex AboutBlank = new(
            Regex.Escape("Expected driver.Url not to be \"about:blank\"."),
            RegexOptions.Compiled);

        // Appium issue where when switching to home webview it becomes unreachable
        private static readonly Regex WindowHandleNotFound = new(
            @"Web window handle not found after 00:00:20; Handles:*",
            RegexOptions.Compiled);

        // Ios issue where the file is not there for the upload test
        private static readonly Regex UnableToFindFileIos = new(
            @"No IOSElement found matching ByIosNSPredicate\(type == 'XCUIElementTypeCell' AND name == 'test, txt'\)*",
            RegexOptions.Compiled);

        // There are UI variances between devices which make the tests flaky though the functionality works
        // The variances will be addressed under NHSO-16258
        private static readonly Regex FileUploadSelectorDeviceVariance = new(
            @"No (?:IOS|Android)Element found matching By(?:IosNSPredicate|AndroidUIAutomator)\(.*?(?:label == 'Done'|name == 'test, txt'|text\(\\?""NhsAppLogo.png\\?""\)|textMatches\(\\?""Images\\?""\))\)",
            RegexOptions.Compiled);

        // kAXErrorServerNotFound - iOS issue throws Appium error: " No webviews found"
        private static readonly Regex KAXErrorServerNotFound = new(
            "kAXErrorServerNotFound",
            RegexOptions.Compiled);

        // Connection failures are causing this screen to show and fail the test
        private static readonly Regex DeviceFailedVersionCheck = new(
            Regex.Escape(UnableToVerifyAppVersion),
            RegexOptions.Compiled);

        // Connection failures are causing this screen to show and fail the test
        private static readonly Regex GoogleServicesFailure = new(
            Regex.Escape(BrowserStackGoogleServicesFailure),
            RegexOptions.Compiled);

        // Web context not ready
        private static readonly Regex WebContextNotReady = new(
            @"Web context not ready after \d[0-9]:\d[0-9]:\d[0-9]: Unable to communicate to node*",
            RegexOptions.Compiled);

        // Unused web context not found webview
        private static readonly Regex UnusedWebContext = new(
            @"Unused web context not found after \d[0-9]:\d[0-9]:\d[0-9]; Contexts: *",
            RegexOptions.Compiled);

        // File upload problems that can be re-ran
        private static readonly Regex CannotFindFileSelectedInUploadScreen = new(
            @"(?=.*No IWebElement found matching\b)(?=.*File selected\b).*",
            RegexOptions.Compiled);

        // Photo capture problems that can be re-ran
        private static readonly Regex CannotFindPhotoCaptured = new(
            @"(?=.*No IWebElement found matching\b)(?=.*Photo captured\b).*",
            RegexOptions.Compiled);

        private static readonly Regex CannotFindPhotosLabelInPhotoScreen = new(
            @"No IOSElement found matching ByIosNSPredicate\(type == 'XCUIElementTypeStaticText' AND label == 'Photos'\)*",
            RegexOptions.Compiled);

        private static readonly Regex CannotFindPhotosButtonInPhotoScreen = new(
            @"No IOSElement found matching ByIosNSPredicate\(type == 'XCUIElementTypeButton' AND label == 'Photos'\)*",
            RegexOptions.Compiled);

        // Issue where it is trying to download photos from cloud
        private static readonly Regex CannotChooseButtonInPhotoScreen = new(
            @"No IOSElement found matching ByIosNSPredicate\(type == 'XCUIElementTypeButton' AND label == 'Choose'\)*",
            RegexOptions.Compiled);

        // Issue where file is not uploaded and we fail the test
        private static readonly Regex BrowserstackDidNotUploadFile = new(
            @"Test could not find test file for upload*",
            RegexOptions.Compiled);

        private static readonly List<(Regex pattern, RetryStatus result)> RetryExceptionMessageRegexes = new()
        {
            (InvalidServiceWebInspectorMessage, RetryStatus.Retry(nameof(InvalidServiceWebInspectorMessage))),
            (AdbErrorListenerNotFound, RetryStatus.Retry(nameof(AdbErrorListenerNotFound))),
            (FailedToCreateDriver, RetryStatus.Retry(nameof(FailedToCreateDriver))),
            (FailedToUpdateNetworkState, RetryStatus.Retry(nameof(FailedToUpdateNetworkState))),
            (DeviceTimeSkew, RetryStatus.Retry(nameof(DeviceTimeSkew))),
            (AppNotRunning, RetryStatus.Retry(nameof(AppNotRunning))),
            (IncorrectChromeVersion, RetryStatus.Retry(nameof(IncorrectChromeVersion))),
            (JavascriptLoadFailure, RetryStatus.Retry(nameof(JavascriptLoadFailure))),
            (AtomExecutionTimeout, RetryStatus.Retry(nameof(AtomExecutionTimeout))),
            (UnableToAccessCamera, RetryStatus.Retry(nameof(UnableToAccessCamera))),
            (NginxBadGateway, RetryStatus.Retry(nameof(NginxBadGateway))),
            (FirebaseAuthorisationFailureWontRetry, RetryStatus.Retry(nameof(FirebaseAuthorisationFailureWontRetry))),
            (AppiumProxyIssue, RetryStatus.Retry(nameof(AppiumProxyIssue))),
            (AppiumUnableToTranslateWebCoordinates, RetryStatus.Retry(nameof(AppiumUnableToTranslateWebCoordinates))),
            (AboutBlank, RetryStatus.Retry(nameof(AboutBlank))),
            (WindowHandleNotFound, RetryStatus.Retry(nameof(WindowHandleNotFound))),
            (UnableToFindFileIos, RetryStatus.Retry(nameof(UnableToFindFileIos))),
            (FileUploadSelectorDeviceVariance, RetryStatus.Retry(nameof(FileUploadSelectorDeviceVariance))),
            (KAXErrorServerNotFound, RetryStatus.Retry(nameof(KAXErrorServerNotFound))),
            (DeviceFailedVersionCheck, RetryStatus.Retry(nameof(DeviceFailedVersionCheck))),
            (UnusedWebContext, RetryStatus.Retry(nameof(UnusedWebContext))),
            (WebContextNotReady, RetryStatus.Retry(nameof(WebContextNotReady))),
            (CannotFindFileSelectedInUploadScreen, RetryStatus.Retry(nameof(CannotFindFileSelectedInUploadScreen))),
            (CannotFindPhotosLabelInPhotoScreen, RetryStatus.Retry(nameof(CannotFindPhotosLabelInPhotoScreen))),
            (CannotFindPhotosButtonInPhotoScreen, RetryStatus.Retry(nameof(CannotFindPhotosButtonInPhotoScreen))),
            (CannotChooseButtonInPhotoScreen, RetryStatus.Retry(nameof(CannotChooseButtonInPhotoScreen))),
            (CannotFindPhotoCaptured, RetryStatus.Retry(nameof(CannotFindPhotoCaptured))),
            (AppiumProxyIssuePostElement, RetryStatus.Retry(nameof(AppiumProxyIssuePostElement))),
            (GoogleServicesFailure, RetryStatus.Retry(nameof(GoogleServicesFailure))),
            (BrowserstackDidNotUploadFile, RetryStatus.Retry(nameof(BrowserstackDidNotUploadFile)))
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