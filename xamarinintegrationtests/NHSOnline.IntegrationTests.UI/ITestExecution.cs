using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    internal interface ITestExecution
    {
        TestResult Execute(TestLogs logs, ITestMethod testMethod);
    }

}