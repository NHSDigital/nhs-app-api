using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace NHSOnline.IntegrationTests.UI
{
    public class DelayExecutionTimer
    {
        private readonly Task _delayTask;

        public DelayExecutionTimer(TimeSpan delay)
        {
            _delayTask = Task.Delay(delay);
        }

        public void Wait()
        {
            _delayTask.Wait();
        }
    }
}