using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NHSOnline.App
{
    internal static class TaskExtensions
    {
        internal static ConfiguredTaskAwaitable PreserveThreadContext(this Task task)
        {
            return task.ConfigureAwait(true);
        }

        internal static ConfiguredTaskAwaitable ResumeOnThreadPool(this Task task)
        {
            return task.ConfigureAwait(false);
        }
    }
}