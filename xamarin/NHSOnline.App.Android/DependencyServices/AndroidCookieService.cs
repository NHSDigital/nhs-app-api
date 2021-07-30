using System.Threading.Tasks;
using System;
using Android.Webkit;
using NHSOnline.App.Api.Client.Cookies;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Threading;
using Xamarin.Forms;
using CookieManager = Android.Webkit.CookieManager;

[assembly: Dependency(typeof(AndroidCookieService))]
namespace NHSOnline.App.Droid.DependencyServices
{

    public class AndroidCookieService : ICookieService
    {
        public async Task SetCookie(ApiCookie apiCookie)
        {
            var cookieManager = CookieManager.Instance ?? throw new InvalidOperationException("Could not get instance of Android CookieManager");

            var taskCompletionSource = new TaskCompletionSource<bool?>();
            using var callBack = new CallBackResult(taskCompletionSource);

            cookieManager.SetCookie(apiCookie.Uri.ToString(), apiCookie.Value, callBack);

            await taskCompletionSource.Task.ResumeOnThreadPool();
        }

        private class CallBackResult : Java.Lang.Object, IValueCallback
        {
            private readonly TaskCompletionSource<bool?> _taskCompletionSource;

            public CallBackResult(TaskCompletionSource<bool?> taskCompletionSource) : base()
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public CallBackResult(IntPtr callBackResult, Android.Runtime.JniHandleOwnership ownership) : base(callBackResult, ownership)
            {
                _taskCompletionSource = new TaskCompletionSource<bool?>();
            }

            public void OnReceiveValue(Java.Lang.Object? value)
            {
                var result = Boolean.TryParse(value?.ToString(), out var parsedBool);
                _taskCompletionSource.SetResult(result && parsedBool);
            }
        }
    }
}