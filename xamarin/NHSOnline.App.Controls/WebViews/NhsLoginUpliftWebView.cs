using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Controls.WebViews.Payloads.Paycasso;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsLoginUpliftWebView : WebView
    {
        public static readonly BindableProperty SelectMediaCommandProperty =
            BindableProperty.Create(
                nameof(SelectMediaCommandProperty),
                typeof(AsyncCommand<ISelectMediaRequest>),
                typeof(NhsLoginUpliftWebView));

        public static readonly BindableProperty LaunchPaycassoCommandProperty =
            BindableProperty.Create(
                nameof(LaunchPaycassoCommandProperty),
                typeof(AsyncCommand<LaunchPaycassoRequest>),
                typeof(NhsLoginUpliftWebView));

        public AsyncCommand<ISelectMediaRequest> SelectMediaCommand
        {
            get => (AsyncCommand<ISelectMediaRequest>) GetValue(SelectMediaCommandProperty);
            set => SetValue(SelectMediaCommandProperty, value);
        }

        public AsyncCommand<LaunchPaycassoRequest> LaunchPaycassoCommand
        {
            get => (AsyncCommand<LaunchPaycassoRequest>)GetValue(LaunchPaycassoCommandProperty);
            set => SetValue(LaunchPaycassoCommandProperty, value);
        }
    }
}