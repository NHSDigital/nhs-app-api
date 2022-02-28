using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    internal class ManualTestExecution: ITestExecution
    {

        private readonly NhsAppManualTestAttribute _attribute;

        public ManualTestExecution(NhsAppManualTestAttribute attribute)
        {
            _attribute = attribute;
        }

        public TestResult Execute(TestLogs logs, ITestMethod testMethod)
        {
            logs.ManualTest(_attribute.ZephyrId, _attribute.Justification);
            return new TestResult
            {
                DisplayName = testMethod.TestMethodName,
                Outcome = UnitTestOutcome.Inconclusive,
            };
        }

        public TestResult Execute(TestLogs logs, ITestMethod testMethod, string parentJourney)
        {
            throw new System.NotImplementedException();
        }

        public TestResult Execute(TestLogs logs, ITestMethod testMethod, bool isFlipbookTest, string parentJourney)
        {
            throw new System.NotImplementedException();
        }
    }
}