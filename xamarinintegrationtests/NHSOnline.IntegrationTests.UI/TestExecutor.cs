using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Reporting;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class TestExecutor
    {
        private readonly string _displayName;
        private readonly ITestMethod _testMethod;
        private readonly ITestExecution _testExecution;

        public TestExecutor(
            string displayName,
            ITestMethod testMethod,
            ITestExecution testExecution)
        {
            _displayName = displayName;
            _testMethod = testMethod;
            _testExecution = testExecution;
        }

        internal TestResult[] Execute()
        {
            var results = new List<TestResult>();
            var retryStatus = RetryStatus.NoRetry;

            do
            {
                if (retryStatus.ShouldRetry)
                {
                    results[^1].Outcome = UnitTestOutcome.Inconclusive;
                }

                var testReport = new TestReport(_testMethod);
                var logs = new TestLogs(testReport);
                logs.Info(_displayName);

                // Include driver setup/teardown in test duration
                var timer = Stopwatch.StartNew();

                var testResult = _testExecution.Execute(logs, _testMethod);

                timer.Stop();

                retryStatus = testResult.ShouldRetry(logs);

                logs.Info("{0} => {1} - {2}", _displayName, testResult.Outcome, retryStatus);

                logs.UpdateResult(testResult, retryStatus);

                testResult.DisplayName = _displayName;
                testResult.Duration = timer.Elapsed;

                results.Add(testResult);
            } while (results.Count < 5 && retryStatus.ShouldRetry);

            return results.ToArray();
        }

    }
}