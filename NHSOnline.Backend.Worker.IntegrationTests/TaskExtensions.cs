using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.IntegrationTests
{
    public static class TaskExtensions
    {

        public static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            var timeoutTask = Task.Delay(timeout).ContinueWith(_ => default(TResult), TaskContinuationOptions.ExecuteSynchronously);
            return Task.WhenAny(task, timeoutTask).Unwrap();
        }
    }
}
