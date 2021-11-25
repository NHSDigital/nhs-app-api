using System;

namespace NHSOnline.IntegrationTests.UI
{
    public class TransitoryErrorHandler
    {
        private TransitoryErrorHandler()
        {
        }

        public void Retry(Action actionToRetry, string errorMessageToTriggerRetry,
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

        public void Alternate(Action action, string errorMessageToTriggerAlternate,
            Action alternateAction)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
                when (e.Message.Contains(
                    errorMessageToTriggerAlternate,
                    StringComparison.InvariantCulture))
            {
                alternateAction.Invoke();
            }
        }

        public static TransitoryErrorHandler HandleSpecificFailure() => new TransitoryErrorHandler();
    }
}