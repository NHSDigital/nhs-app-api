using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Reporting;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class TestExecutor<TDriverWrapper> where TDriverWrapper: IDriverWrapper
    {
        private readonly string _displayName;
        private readonly ITestMethod _testMethod;
        private readonly Func<TestLogs, TestTempDirectory, TDriverWrapper> _createDriverWrapper;

        public TestExecutor(
            string displayName,
            ITestMethod testMethod,
            Func<TestLogs, TestTempDirectory, TDriverWrapper> driverWrapper)
        {
            _displayName = displayName;
            _testMethod = testMethod;
            _createDriverWrapper = driverWrapper;
        }

        internal TestResult[] Execute()
        {
            var results = new List<TestResult>();
            var shouldRetry = false;

            do
            {
                if (shouldRetry)
                {
                    results[^1].Outcome = UnitTestOutcome.Inconclusive;
                }

                var testReport = new TestReport(_testMethod);
                var logs = new TestLogs(testReport);
                logs.Info(_displayName);

                // Include driver setup/teardown in test duration
                var timer = Stopwatch.StartNew();

                var testResult = ExecuteInternal(logs);

                timer.Stop();

                shouldRetry = testResult.ShouldRetry(logs);
                testReport.ShouldRetry = shouldRetry;

                logs.Info("{0} => {1}{2}", _displayName, testResult.Outcome, shouldRetry ? " - Should Retry" : string.Empty);

                logs.UpdateResult(testResult);

                testResult.DisplayName = _displayName;
                testResult.Duration = timer.Elapsed;

                results.Add(testResult);
            } while (results.Count < 5 && shouldRetry);

            return results.ToArray();
        }

        private TestResult ExecuteInternal(TestLogs logs)
        {
            if (_testMethod.HasInvalidParameters<TDriverWrapper>(out var errorResult))
            {
                return errorResult;
            }

            try
            {
                var tempDirectory = new TestTempDirectory();
                using var driver = _createDriverWrapper(logs, tempDirectory);
                using var context = new TestContext(_testMethod, logs, tempDirectory, driver);

                var testResult = _testMethod.Invoke(new object[] {driver});

                context.Cleanup(testResult);

                return testResult;
            }
            catch (Exception e)
            {
                logs.Error("Execute Test Failed: {0}", e);
                return new TestResult
                {
                    DisplayName = _testMethod.TestMethodName,
                    Outcome = UnitTestOutcome.NotRunnable,
                    TestFailureException = e
                };
            }
        }
    }
}