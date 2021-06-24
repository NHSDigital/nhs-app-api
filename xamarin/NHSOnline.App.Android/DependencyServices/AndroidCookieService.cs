using System.Net;
using System.Threading.Tasks;
using System;
using Android.Webkit;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Threading;
using Xamarin.Forms;
using Task = System.Threading.Tasks.Task;

[assembly: Dependency(typeof(AndroidCookieService))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidCookieService : ICookieService
    {
        public async Task SetCookie(Cookie cookie)
        {
            var cookieManager = CookieManager.Instance ?? throw new InvalidOperationException("CookieManager.Instance was null");

            var taskCompletionSource = new TaskCompletionSource<bool?>();
            using var callBack = new CallBackResult(taskCompletionSource);

            cookieManager.SetCookie(cookie.Domain, cookie.ToString(), callBack);

            await taskCompletionSource.Task.ResumeOnThreadPool();
        }

        public class CallBackResult : Java.Lang.Object, IValueCallback
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
                _taskCompletionSource.SetResult(true);
            }
        }
    }
}