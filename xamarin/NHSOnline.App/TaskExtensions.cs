using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NHSOnline.App
{
    // Provide alternative names for ConfigureAwait which describe effect
    // of the continueOnCapturedContext parameter.
    // See: https://devblogs.microsoft.com/dotnet/configureawait-faq/
    internal static class TaskExtensions
    {
        internal static ConfiguredTaskAwaitable PreserveThreadContext(this Task task)
        {
            return task.ConfigureAwait(true);
        }

        internal static ConfiguredTaskAwaitable<T> PreserveThreadContext<T>(this Task<T> task)
        {
            return task.ConfigureAwait(true);
        }

        internal static ConfiguredTaskAwaitable ResumeOnThreadPool(this Task task)
        {
            return task.ConfigureAwait(false);
        }

        internal static ConfiguredTaskAwaitable<T> ResumeOnThreadPool<T>(this Task<T> task)
        {
            return task.ConfigureAwait(false);
        }
    }
}