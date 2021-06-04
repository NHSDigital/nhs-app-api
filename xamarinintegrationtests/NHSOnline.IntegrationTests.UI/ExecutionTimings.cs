using System;

namespace NHSOnline.IntegrationTests.UI
{
    public class ExecutionTiming
    {
        public DateTime StartTime { get; }
        public DateTime StopTime { get; private set; }

        internal ExecutionTiming(DateTime startTime)
        {
            StartTime = startTime;
        }

        internal void Stop()
        {
            StopTime = DateTime.UtcNow;
        }
    }
}