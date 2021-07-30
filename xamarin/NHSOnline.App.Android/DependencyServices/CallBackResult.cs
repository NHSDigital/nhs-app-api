using System;
using System.Threading.Tasks;
using Android.Webkit;

namespace NHSOnline.App.Droid.DependencyServices
{
    internal class CallBackResult : Java.Lang.Object, IValueCallback
    {
        private readonly TaskCompletionSource<bool> _taskCompletionSource;

        public CallBackResult()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
        }

        void IValueCallback.OnReceiveValue(Java.Lang.Object? value)
        {
            var result = Boolean.TryParse(value?.ToString(), out var parsedBool);
            _taskCompletionSource.SetResult(result && parsedBool);
        }

        public Task<bool> GetAwaitable() => _taskCompletionSource.Task;
    }
}