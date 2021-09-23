using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    public class KnownIssue
    {
        private bool AssertedExpectedBehaviour { get; set; }
        private string FailureMessage { get; }

        private KnownIssue(string failureMessage)
        {
            FailureMessage = failureMessage;
        }

        public KnownIssue ShouldExpect(Action assertion)
        {
            try
            {
                assertion.Invoke();
                AssertedExpectedBehaviour = true;
            }
            catch (AssertFailedException)
            {
            }

            return this;
        }

        public void OrIfKnownIssueOccuredExpect(Action assertion)
        {
            if (AssertedExpectedBehaviour)
            {
                return;
            }

            assertion.Invoke();

            throw new AssertFailedException(FailureMessage);
        }

        public static KnownIssue BrowserStackNetworkChangeFailed()
            => new KnownIssue(TestResultRetryExtensions.FailedToUpdateNetworkStateMessage);
    }
}