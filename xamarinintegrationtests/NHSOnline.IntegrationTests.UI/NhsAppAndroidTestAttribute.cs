using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.Native.Android;

namespace NHSOnline.IntegrationTests.UI
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NhsAppAndroidTestAttribute : TestMethodAttribute
    {
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var testName = DisplayName ?? testMethod.TestMethodName;

            var testExecution = new AutomatedTestExecution<IAndroidDriverWrapper>(
                logs => new AndroidDriverWrapper(testName, logs));

            var testExecutor = new TestExecutor(testName, testMethod, testExecution);

            return testExecutor.Execute();
        }
    }
}