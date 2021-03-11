using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.Native.IOS;

namespace NHSOnline.IntegrationTests.UI
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NhsAppIOSTestAttribute : TestMethodAttribute
    {
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var testName = DisplayName ?? testMethod.TestMethodName;

            var testExecution = new AutomatedTestExecution<IIOSDriverWrapper>(
                logs => new IOSDriverWrapper(testName, logs));

            var testExecutor = new TestExecutor(testName, testMethod, testExecution);

            return testExecutor.Execute();
        }
    }
}