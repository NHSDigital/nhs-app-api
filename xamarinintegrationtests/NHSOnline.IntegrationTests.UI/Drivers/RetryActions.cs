using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public static class RetryActions
    {
        public static void RetryAssertion(TimeSpan timeSpan, Action action)
        {
            var retryUntil = DateTime.UtcNow.Add(timeSpan);

            while (true)
            {
                try
                {
                    action();
                    return;
                }
                catch (AssertFailedException) when (DateTime.UtcNow < retryUntil)
                {
                    // Retry within the time period
                }
            }
        }
    }
}