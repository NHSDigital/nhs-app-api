using Android.Views.Accessibility;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class AccessibilityWebViewRendererExtension: IWebViewRendererExtension
    {
        private readonly WebViewRenderer _renderer;

        public AccessibilityWebViewRendererExtension(WebViewRenderer renderer)
            => _renderer = renderer;

        public void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement == null &&
                _renderer.Control.Settings != null &&
                _renderer.Element is IAccessibleWebView view)
            {
                view.AccessibilityFocusChangeRequested += (sender, args) =>
                {
                    _renderer.Control.SendAccessibilityEvent(EventTypes.ViewAccessibilityFocused);
                };
            }
        }

    }
}