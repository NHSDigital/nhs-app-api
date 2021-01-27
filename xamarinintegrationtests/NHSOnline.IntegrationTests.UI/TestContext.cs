using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class TestContext : IDisposable
    {

        private readonly ITestMethod _testMethod;
        private readonly TestTempDirectory _testTempDirectory;
        private readonly IDriverWrapper _driver;
        private readonly DockerLogs _dockerLogs;
        private readonly MocksLogs _mocksLogs;

        internal TestContext(
            ITestMethod testMethod,
            TestLogs logs,
            TestTempDirectory testTempDirectory,
            IDriverWrapper driver)
        {
            _testMethod = testMethod;
            _testTempDirectory = testTempDirectory;
            _driver = driver;
            _dockerLogs = new DockerLogs(testTempDirectory);
            _mocksLogs = new MocksLogs();

            Logs = logs;
        }

        internal TestLogs Logs { get; }

        internal string TestName => _testMethod.TestMethodName;

        internal FileInfo GetTempFile(string filename) => _testTempDirectory.GetTempFile(filename);

        internal void Cleanup(TestResult testResult)
        {
            var testResultContext = new TestResultContext(this, testResult);

            if (testResult.Outcome != UnitTestOutcome.Passed)
            {
                _driver.AttachDebugInfo(testResultContext);
                _dockerLogs.AttachDockerLogs(testResultContext);
                _mocksLogs.AttachMocksLogs(testResultContext);
                _driver.UpdateBrowserStackStatusToFailed(testResultContext);
            }

            _driver.Cleanup(testResultContext);
        }


        public void Dispose()
        {
            _mocksLogs.Dispose();
        }
    }
}