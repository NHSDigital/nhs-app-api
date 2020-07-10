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
            var testName = "[Android] " + (DisplayName ?? testMethod.TestMethodName);

            var testExecutor = new TestExecutor<IAndroidDriverWrapper>(
                testName,
                testMethod,
                logs => new AndroidDriverWrapper(testName, logs));

            return new[] {testExecutor.Execute()};
        }
    }
}