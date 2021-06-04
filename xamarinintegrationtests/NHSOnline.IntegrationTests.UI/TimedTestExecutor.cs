using System;

namespace NHSOnline.IntegrationTests.UI
{
    public static class TimedTestExecutor
    {
        public static ExecutionTiming Execute(Action test)
        {
            var executionTiming = new ExecutionTiming(DateTime.UtcNow);
            test.Invoke();
            executionTiming.Stop();
            return executionTiming;
        }
    }
}