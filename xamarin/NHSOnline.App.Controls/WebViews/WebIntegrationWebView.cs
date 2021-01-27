using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class WebIntegrationWebView : WebView
    {
        public const string JavascriptObjectName = "nhsappNative";

        public static readonly BindableProperty RedirectToNhsAppPageCommandProperty =
            BindableProperty.Create(nameof(RedirectToNhsAppPageCommand), typeof(AsyncCommand<string>),
                typeof(WebIntegrationWebView));

        public AsyncCommand<string> RedirectToNhsAppPageCommand
        {
            get => (AsyncCommand<string>) GetValue(RedirectToNhsAppPageCommandProperty);
            set => SetValue(RedirectToNhsAppPageCommandProperty, value);
        }
    }
}
