using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NHSOnline.App.Threading
{
    /// <summary>
    /// Provide alternative names for ConfigureAwait which describe affect
    /// of the continueOnCapturedContext parameter.
    /// https://devblogs.microsoft.com/dotnet/configureawait-faq/
    /// </summary>
    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable ResumeOnThreadPool(this Task task)
        {
            return task.ConfigureAwait(false);
        }

        public static ConfiguredValueTaskAwaitable ResumeOnThreadPool(this ValueTask task)
        {
            return task.ConfigureAwait(false);
        }

        public static ConfiguredTaskAwaitable<T> ResumeOnThreadPool<T>(this Task<T> task)
        {
            return task.ConfigureAwait(false);
        }

        public static ConfiguredValueTaskAwaitable<T> ResumeOnThreadPool<T>(this ValueTask<T> task)
        {
            return task.ConfigureAwait(false);
        }
    }
}
