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

            var testExecutor = new TestExecutor<IIOSDriverWrapper>(
                testName,
                testMethod,
                logs => new IOSDriverWrapper(testName, logs));

            return new[] { testExecutor.Execute() };
        }
    }
}