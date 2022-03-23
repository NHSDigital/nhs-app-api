using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests;

[AttributeUsage(AttributeTargets.Method)]
public sealed class NhsAppTestAttribute : TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var testName = DisplayName ?? testMethod.TestMethodName;

        var testExecutor = TestExecutor.Create(testName, testMethod);

        return new[] { testExecutor.Execute() };
    }
}