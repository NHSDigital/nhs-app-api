using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NhsAppAndroidTestAttribute : TestMethodAttribute
    {
        public NhsAppAndroidTestAttribute(string displayName) : base(displayName)
        { }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var testExecutor = new TestExecutor<IAndroidDriverWrapper>(
                DisplayName,
                testMethod,
                logs => new AndroidDriverWrapper(DisplayName, logs));

            return new[] {testExecutor.Execute()};
        }
    }
}