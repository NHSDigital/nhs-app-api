using NhsAppAnalytics.FunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;

internal sealed class TestExecutor
    {
        private readonly string _displayName;
        private readonly ITestMethod _testMethod;

        private readonly TestExecutorImplBase _testExecutorImpl;

        private TestExecutor(
            string displayName,
            ITestMethod testMethod,
            TestExecutorImplBase testExecutorImpl)
        {
            _displayName = displayName;
            _testMethod = testMethod;
            _testExecutorImpl = testExecutorImpl;
        }

        internal static TestExecutor Create(string displayName, ITestMethod testMethod)
        {
            return new TestExecutor(displayName, testMethod, new TestExecutorImpl(testMethod));
        }

        internal static TestExecutor Create<TSourceWrapper>(string displayName, ITestMethod testMethod)
            where TSourceWrapper : ISourceWrapper, new()
        {
            return new TestExecutor(displayName, testMethod, new TestExecutorImpl<TSourceWrapper>(testMethod));
        }

        internal TestResult Execute()
        {
            var logs = new TestLogs();
            logs.Info(_displayName);

            // Include driver setup/teardown in test duration
            var timer = Stopwatch.StartNew();

            var testResult = AsyncHelper.RunSync(async () => await ExecuteInternal(logs));

            logs.Info("{0} => {1}", _displayName, testResult.Outcome);
            logs.UpdateResult(testResult);

            timer.Stop();
            testResult.DisplayName = _displayName;
            testResult.Duration = timer.Elapsed;

            return testResult;
        }

        private async Task<TestResult> ExecuteInternal(TestLogs logs)
        {
            if (_testExecutorImpl.HasInvalidParameters(out var errorResult))
            {
                return errorResult;
            }

            try
            {
                using var testEnv = new TestEnv(logs);
                await testEnv.Initialise();

                var testResult = await _testExecutorImpl.Invoke(testEnv, logs);

                var testResultContext = new TestResultContext(testEnv, testResult);
                await testEnv.CleanUp(testResultContext);

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

        private abstract class TestExecutorImplBase
        {
            protected internal abstract bool HasInvalidParameters(out TestResult errorResult);

            protected internal abstract Task<TestResult> Invoke(TestEnv testEnv, TestLogs testLogs);
        }

        private sealed class TestExecutorImpl : TestExecutorImplBase
        {
            private readonly ITestMethod _testMethod;

            public TestExecutorImpl(ITestMethod testMethod)
                => _testMethod = testMethod;

            protected internal override bool HasInvalidParameters(out TestResult errorResult)
                => _testMethod.HasInvalidParameters(out errorResult);

            protected internal override Task<TestResult> Invoke(TestEnv testEnv, TestLogs testLogs)
            {
                var testResult = _testMethod.Invoke(new object[] {testEnv});
                return Task.FromResult(testResult);
            }
        }

        private sealed class TestExecutorImpl<TSourceWrapper> : TestExecutorImplBase
            where TSourceWrapper : ISourceWrapper, new()
        {
            private readonly ITestMethod _testMethod;

            public TestExecutorImpl(ITestMethod testMethod)
                => _testMethod = testMethod;

            protected internal override bool HasInvalidParameters(out TestResult errorResult)
                => _testMethod.HasInvalidParameters<TSourceWrapper>(out errorResult);

            protected internal override async Task<TestResult> Invoke(TestEnv testEnv, TestLogs testLogs)
            {
                var sourceWrapper = new TSourceWrapper();
                await sourceWrapper.Initialise(testLogs);

                return _testMethod.Invoke(new object[] {sourceWrapper, testEnv});
            }
        }
    }