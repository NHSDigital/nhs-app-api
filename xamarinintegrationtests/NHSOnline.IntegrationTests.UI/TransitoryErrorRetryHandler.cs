using System;

namespace NHSOnline.IntegrationTests.UI
{
    public class TransitoryErrorRetryHandler
    {
        private TransitoryErrorRetryHandler() {}

        public void Handle(Action actionToRetry, string errorMessageToTriggerRetry,
            Action actionToRevertTestState)
        {
            try
            {
                actionToRetry.Invoke();
            }
            catch (Exception e)
                when (e.Message.Contains(
                    errorMessageToTriggerRetry,
                    StringComparison.InvariantCulture))
            {
                actionToRevertTestState.Invoke();

                actionToRetry.Invoke();
            }
        }

        public static TransitoryErrorRetryHandler RetryOnSpecificFailure() => new TransitoryErrorRetryHandler();
    }
}