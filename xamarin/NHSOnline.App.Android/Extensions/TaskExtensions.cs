using System.Threading.Tasks;
using Android.Gms.Tasks;
using Java.Lang;
using Microsoft.Extensions.Logging;
using Task = Android.Gms.Tasks.Task;

namespace NHSOnline.App.Droid.Extensions
{
    public static class TaskExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2000:Dispose objects before losing scope", Justification = "Throws exception otherwise, see https://stackoverflow.com/questions/26573627/no-constructor-found-for-system-intptr-android-runtime-jnihandleownership")]
        public static Task<Object> ToAwaitableTask(this Task task, ILogger logger)
        {
            var taskCompletionSource = new TaskCompletionSource<Java.Lang.Object>();
            var taskCompleteListener = new TaskCompleteListener(taskCompletionSource, logger);
            task.AddOnCompleteListener(taskCompleteListener);

            return taskCompletionSource.Task;
        }

        private class TaskCompleteListener : Java.Lang.Object, IOnCompleteListener
        {
            private readonly TaskCompletionSource<Java.Lang.Object> _taskCompletionSource;
            private readonly ILogger _logger;

            public TaskCompleteListener(TaskCompletionSource<Object> tcs, ILogger logger)
            {
                _taskCompletionSource = tcs;
                _logger = logger;
            }

            public void OnComplete(Android.Gms.Tasks.Task task)
            {
                if (task.IsCanceled)
                {
                    _logger.LogError("Android TaskCompleteListener OnComplete Event handler - Task was Cancelled");
                    _taskCompletionSource.SetCanceled();
                }
                else if (task.IsSuccessful)
                {
                    _taskCompletionSource.SetResult(task.Result);
                }
                else
                {
                    _logger.LogError("Android TaskCompleteListener OnComplete Event handler - Task has Errored: {@TaskCompleteError}",task.Exception);
                    _taskCompletionSource.SetException(task.Exception);
                }
            }
        }
    }
}