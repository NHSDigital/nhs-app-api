using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NhsAppWebTestAttribute: TestMethodAttribute
    {
        public NhsAppWebTestAttribute(string displayName) : base(displayName)
        { }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var testExecutor = new TestExecutor<IWebDriverWrapper>(
                DisplayName,
                testMethod,
                logs => new ChromeWebDriverWrapper(logs));

            return new[] { testExecutor.Execute() };
        }
    }
}
