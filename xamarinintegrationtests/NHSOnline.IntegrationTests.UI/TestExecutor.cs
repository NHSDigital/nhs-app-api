using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;

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

        internal TestResult Execute()
        {
            var logs = new TestLogs();
            logs.Info(_displayName);

            // Include driver setup/teardown in test duration
            var timer = Stopwatch.StartNew();

            var testResult = ExecuteInternal(logs);

            logs.Info("{0} => {1}", _displayName, testResult.Outcome);
            logs.UpdateResult(testResult);

            timer.Stop();
            testResult.DisplayName = _displayName;
            testResult.Duration = timer.Elapsed;

            return testResult;
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
                var context = new TestContext(_testMethod, logs, tempDirectory, driver);

                var testResult = _testMethod.Invoke(new object[] { driver });

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