using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.Web.Chrome;

namespace NHSOnline.IntegrationTests.UI
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NhsAppWebTestAttribute: TestMethodAttribute
    {
        public NhsAppWebTestAttribute(string displayName) : base(displayName)
        { }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var testName = DisplayName ?? testMethod.TestMethodName;

            var testExecutor = new TestExecutor<IWebDriverWrapper>(
                testName,
                testMethod,
                (logs, tempDir) => new ChromeWebDriverWrapper(logs, tempDir));

            return new[] { testExecutor.Execute() };
        }
    }
}
