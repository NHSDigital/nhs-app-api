using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NHSOnline.IntegrationTestAnalyser.Models
{
    public class CustomTestResult
    {
        public CustomTestResult(string testId, string outcome, string runTime)
        {
            TestId = testId;
            Outcome = outcome;
            RunTime = runTime;
            ErrorList = new Collection<string>();
        }

        public string TestId { get; set; }

        public string Outcome { get; set; }

        public string RunTime { get; set; }

        public CustomTestReport? TestReport { get; set; }

        public Collection<string> ErrorList { get; }

        public bool IsManual => TestReport == null || TestReport.IsManual;

        public string Category
        {
            get
            {
                var errorTypes = GetErrorTypeList();

                return errorTypes.FirstOrDefault(x => x.IsMatch(this))?.DisplayName ?? "UNKNOWN";
            }
        }

        private IEnumerable<ErrorType> GetErrorTypeList()
        {
            yield return BasicErrorType.Create("Unable to connect to renderer");
            yield return BasicErrorType.Create("Unable to translate web coordinates for native web tap");
            yield return BasicErrorType.Create("Did not get any response for atom execution");
            yield return BasicErrorType.Create("Error executing adbExec");
            yield return BasicErrorType.Create("chrome not reachable");
            yield return BasicErrorType.Create("Could not start a session. Something went wrong with app launch");
            yield return BasicErrorType.Create("This version of ChromeDriver only supports Chrome version");
            yield return BasicErrorType.Create("A exception with a null response was thrown sending an HTTP request to the remote WebDriver server for URL");
            yield return BasicErrorType.Create("Could not start Mobile Browser");
            yield return BasicErrorType.Create("timed out after 60 seconds");
            yield return BasicErrorType.Create("An attempt was made to operate on a modal dialog when one was not open");
            yield return BasicErrorType.Create("Unused web context not found after");
            yield return BasicErrorType.Create("Web window handle not found after");
            yield return BasicErrorType.Create("Unable to communicate to node");
            yield return BasicErrorType.Create("Web context not ready after 00:01:00");
            yield return BasicErrorType.Create("No IWebElement found matching By.XPath: //h1[normalize-space()='Manage notifications']");
            yield return BasicErrorType.Create("OpenQA.Selenium.NoSuchWindowException: no such window");

            yield return BasicErrorType.Create("Google login failure",
                "Something went wrong with Google login. Please try to run the test again",
                "Captcha prompted during Google login");

            yield return BasicErrorType.Create("Photo picker not working right",
                "No IOSElement found matching ByIosNSPredicate(type == 'XCUIElementTypeButton' AND label == 'Done')",
                "No AndroidElement found matching ByAndroidUIAutomator(new UiSelector().className(\"android.widget.TextView\").textMatches(\"Images\"))");

            yield return BasicErrorType.Create("Probably internet wasn't turned off as expected",
                "No IOSElement found matching ByIosNSPredicate(type == 'XCUIElementTypeStaticText' AND value == 'Unable to verify app version')");

            yield return BasicErrorType.Create("iOS session expiry white screen",
                "No IOSElement found matching ByIosNSPredicate(type == 'XCUIElementTypeStaticText' AND value == \"For security reasons, we'll log you out of the NHS App in 1 minute.\"",
                "No IOSElement found matching ByIosNSPredicate(type == 'XCUIElementTypeButton' AND label == 'Stay logged in')",
                "Expected context.Element.Displayed to be true because a label with text 'For security reasons, we'll log you out of the NHS App in 1 minute.' should be displayed, but found False");

            yield return SpecificTestErrorType.Create("Deep link didn't launch the app", new[] { "ClosedAppDeepLink" },
                new[]
                {
                    "No AndroidElement found matching ByAndroidUIAutomator(new UiSelector().className(\"android.widget.TextView\").text(\"Access your NHS services\"))"
                });

            yield return BasicErrorType.Create("Probably Android calendar didn't open the edit screen",
                "No AndroidElement found matching By.XPath: .//android.widget.EditText[normalize-space(@text)='Test Subject']");

            yield return BasicErrorType.Create("[browserstack.local] is set to true but local testing through BrowserStack is not connected");

            yield return BasicErrorType.Create("iOS didn't prompt for calendar access", "No IOSElement found matching ByIosNSPredicate(type == 'XCUIElementTypeStaticText' AND value MATCHES 'Give NHS App calendar access')");
        }

        abstract class ErrorType
        {
            internal abstract string DisplayName { get; }
            internal abstract bool IsMatch(CustomTestResult testResult);
        }

        class BasicErrorType : ErrorType
        {
            private readonly string[] _possibleErrorStrings;

            private BasicErrorType(string displayName, string[] possibleErrorStrings)
            {
                _possibleErrorStrings = possibleErrorStrings;
                DisplayName = displayName;
            }

            internal static BasicErrorType Create(string error)
            {
                return new BasicErrorType(error, new[] { error });
            }

            internal static BasicErrorType Create(string displayName, params string[] possibleErrorStrings)
            {
                return new BasicErrorType(displayName, possibleErrorStrings);
            }

            internal override string DisplayName { get; }
            internal override bool IsMatch(CustomTestResult testResult)
            {
                foreach (var possibleErrorString in _possibleErrorStrings)
                {
                    if (testResult.ErrorList.Any(testError =>
                        testError.Contains(possibleErrorString, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        class SpecificTestErrorType : ErrorType
        {
            private readonly string[] _testNames;
            private readonly BasicErrorType _basicErrorType;

            private SpecificTestErrorType(string displayName, string[] testNames, string[] possibleErrorStrings)
            {
                _testNames = testNames;
                _basicErrorType = BasicErrorType.Create(displayName, possibleErrorStrings);
            }

            public static SpecificTestErrorType Create(string displayName, string[] testNames, string[] possibleErrorStrings)
            {
                return new SpecificTestErrorType(displayName, testNames, possibleErrorStrings);
            }

            internal override string DisplayName => _basicErrorType.DisplayName;
            internal override bool IsMatch(CustomTestResult testResult)
            {
                if (_testNames.Any(x => x.Equals(testResult.TestReport?.MethodName, StringComparison.OrdinalIgnoreCase)))
                {
                    return _basicErrorType.IsMatch(testResult);
                }

                return false;
            }
        }
    }
}