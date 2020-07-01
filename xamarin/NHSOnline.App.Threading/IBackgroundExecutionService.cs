using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Threading
{
    public interface IBackgroundExecutionService
    {
        Task Run(Func<Task> action);
        Task<T> Run<T>(Func<Task<T>> func);
    }
}