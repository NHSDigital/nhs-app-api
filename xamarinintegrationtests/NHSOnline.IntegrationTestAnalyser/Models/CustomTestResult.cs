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
                string[] errorList =
                {
                    "Appium error: An unknown server-side error occurred while processing the command. Original error: disconnected: unable to connect to renderer",
                    "Appium error: An unknown server-side error occurred while processing the command. Original error: No webviews found. Unable to translate web coordinates for native web tap",
                    "Appium error: An unknown server-side error occurred while processing the command. Original error: Did not get any response for atom execution",
                    "Appium error: An unknown server-side error occurred while processing the command. Original error: Error executing adbExec",
                    "Appium error: An unknown server-side error occurred while processing the command. Original error: chrome not reachable",
                    "Could not start a session. Something went wrong with app launch. Please try to run the test again.",
                    "This version of ChromeDriver only supports Chrome version",
                    "A exception with a null response was thrown sending an HTTP request to the remote WebDriver server for URL",
                    "Could not start Mobile Browser",
                    "timed out after 60 seconds",
                    "Web context not ready",
                    "An attempt was made to operate on a modal dialog when one was not open",
                    "Something went wrong with Google login. Please try to run the test again.",
                    "Unused web context not found after",
                    "Web window handle not found after",
                    "Unable to communicate to node",
                    "Web context not ready after 00:01:00"
                };

                foreach (var currentError in errorList)
                {
                    if (ErrorList.Any(error =>
                        error.Contains(currentError, StringComparison.OrdinalIgnoreCase)))
                        return currentError;
                }


                return "UNKNOWN";
            }
        }
    }
}