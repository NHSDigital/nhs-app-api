using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NhsAppManualTestAttribute : TestMethodAttribute
    {

        public string ZephyrId { get; }
        public string Justification { get; }

        public NhsAppManualTestAttribute(string zephyrId, string justification)
        {
            ZephyrId = zephyrId;
            Justification = justification;
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var testName = DisplayName ?? testMethod.TestMethodName;
            var testExecution = new ManualTestExecution(this);
            var testExecutor = new TestExecutor(testName,  testMethod, testExecution);

            return testExecutor.Execute();
        }
    }
}