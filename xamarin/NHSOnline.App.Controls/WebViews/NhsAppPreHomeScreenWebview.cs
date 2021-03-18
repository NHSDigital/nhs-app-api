using System;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsAppPreHomeScreenWebview: WebView
    {
        public static readonly BindableProperty GetNotificationsStatusCommandProperty =
            BindableProperty.Create(nameof(GetNotificationsStatusCommand), typeof(AsyncCommand), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty RequestPnsTokenCommandProperty =
            BindableProperty.Create(nameof(RequestPnsTokenCommand), typeof(AsyncCommand<string>), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty GoToLoggedInHomeScreenCommandProperty =
            BindableProperty.Create(nameof(GoToLoggedInHomeScreenCommand), typeof(AsyncCommand), typeof(NhsAppPreHomeScreenWebview));

        public Func<Cookie, Task>? SetCookie { get; set; }

        public AsyncCommand GetNotificationsStatusCommand
        {
            get => (AsyncCommand) GetValue(GetNotificationsStatusCommandProperty);
            set => SetValue(GetNotificationsStatusCommandProperty, value);
        }

        public AsyncCommand<string> RequestPnsTokenCommand
        {
            get => (AsyncCommand<string>) GetValue(RequestPnsTokenCommandProperty);
            set => SetValue(RequestPnsTokenCommandProperty, value);
        }

        public AsyncCommand GoToLoggedInHomeScreenCommand
        {
            get => (AsyncCommand) GetValue(GoToLoggedInHomeScreenCommandProperty);
            set => SetValue(GoToLoggedInHomeScreenCommandProperty, value);
        }
    }
}
