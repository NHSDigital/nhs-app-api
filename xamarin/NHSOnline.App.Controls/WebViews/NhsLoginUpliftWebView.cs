using NHSOnline.App.Controls.WebViews.Payloads;
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

        public AsyncCommand<ISelectMediaRequest> SelectMediaCommand
        {
            get => (AsyncCommand<ISelectMediaRequest>) GetValue(SelectMediaCommandProperty);
            set => SetValue(SelectMediaCommandProperty, value);
        }
    }
}