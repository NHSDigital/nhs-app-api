using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NhsAppIOSTestAttribute : TestMethodAttribute
    {
        public NhsAppIOSTestAttribute(string displayName) : base(displayName)
        { }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var testExecutor = new TestExecutor<IIOSDriverWrapper>(
                DisplayName,
                testMethod,
                logs => new IOSDriverWrapper(DisplayName, logs));

            return new[] { testExecutor.Execute() };
        }
    }
}