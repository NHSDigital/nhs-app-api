using Android.Views.Accessibility;
using NHSOnline.App.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class AccessibilityWebViewRendererExtension: WebViewRendererExtension
    {
        private readonly WebViewRenderer _renderer;

        public AccessibilityWebViewRendererExtension(WebViewRenderer renderer)
            => _renderer = renderer;

        internal override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement == null &&
                _renderer.Control.Settings != null &&
                _renderer.Element is IAccessibleControl view)
            {
                view.AccessibilityFocusChangeRequested += (sender, args) =>
                {
                    _renderer.Control.SendAccessibilityEvent(EventTypes.ViewAccessibilityFocused);
                };
            }
        }
    }
}