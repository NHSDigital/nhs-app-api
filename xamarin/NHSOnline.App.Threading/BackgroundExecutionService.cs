using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Threading
{
    internal sealed class BackgroundExecutionService: IBackgroundExecutionService
    {
        public Task Run(Func<Task> action)
        {
            return Task.Run(action);
        }

        public Task<T> Run<T>(Func<Task<T>> func)
        {
            return Task.Run(func);
        }
    }
}