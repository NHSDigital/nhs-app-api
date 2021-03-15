using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI
{
    internal class AutomatedTestExecution<TDriverWrapper> : ITestExecution where TDriverWrapper : IDisposable, IDriverWrapper
    {
        private readonly Func<TestLogs, TDriverWrapper> _createDriverWrapper;

        public AutomatedTestExecution(Func<TestLogs, TDriverWrapper> driverWrapper)
        {
            _createDriverWrapper = driverWrapper;
        }
        public TestResult Execute(TestLogs logs, ITestMethod testMethod)
        {

            if (testMethod.HasInvalidParameters<TDriverWrapper>(out var errorResult))
            {
                return errorResult;
            }

            try
            {
                var tempDirectory = new TestTempDirectory();
                using var driver = CreateDriver(logs);
                using var context = new TestContext(testMethod, logs, tempDirectory, driver);

                var testResult = testMethod.Invoke(new object[] { driver });

                context.Cleanup(testResult);

                return testResult;
            }
            catch (Exception e)
            {
                logs.Error("Execute Test Failed: {0}", e);
                return new TestResult
                {
                    DisplayName = testMethod.TestMethodName,
                    Outcome = UnitTestOutcome.NotRunnable,
                    TestFailureException = e
                };
            }
        }

        private TDriverWrapper CreateDriver(TestLogs logs)
        {
            try
            {
                return _createDriverWrapper(logs);
            }
            catch (WebDriverException e)
            {
                throw new AssertFailedException(TestResultRetryExtensions.FailedToCreateDriverMessage, e);
            }
        }
    }
}