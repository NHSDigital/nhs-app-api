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

        /// <summary>
        /// Should only be used to resume on the UI thread context
        /// </summary>
        public static ConfiguredTaskAwaitable PreserveThreadContext(this Task task)
        {
            return task.ConfigureAwait(true);
        }

        /// <summary>
        /// Should only be used to resume on the UI thread context
        /// </summary>
        public static ConfiguredTaskAwaitable<T> PreserveThreadContext<T>(this Task<T> task)
        {
            return task.ConfigureAwait(true);
        }

        public static ConfiguredValueTaskAwaitable<T> PreserveThreadContext<T>(this ValueTask<T> task)
        {
            return task.ConfigureAwait(true);
        }

        public static ConfiguredValueTaskAwaitable PreserveThreadContext(this ValueTask task)
        {
            return task.ConfigureAwait(true);
        }
    }
}
