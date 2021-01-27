using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NHSOnline.App.Areas
{
    /// <summary>
    /// Provide alternative names for ConfigureAwait which describe affect
    /// of the continueOnCapturedContext parameter.
    /// https://devblogs.microsoft.com/dotnet/configureawait-faq/
    /// </summary>
    /// <seealso cref="Threading.TaskExtensions"/>
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
    }
}