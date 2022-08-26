using System;
using Android.Views.Accessibility;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Logging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class AccessibilityWebViewRendererExtension: WebViewRendererExtension, IDisposable
    {
        private readonly WebViewRenderer _renderer;

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AccessibilityWebViewRendererExtension));

        public AccessibilityWebViewRendererExtension(WebViewRenderer renderer)
            => _renderer = renderer;

        internal override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement == null &&
                _renderer.Control?.Settings != null &&
                _renderer.Element is IAccessibleControl view)
            {
                view.AccessibilityFocusChangeRequested += (sender, args) => SendViewAccessibilityEvent();
            }
        }

        private void SendViewAccessibilityEvent()
        {
            try
            {
                _renderer.Control.SendAccessibilityEvent(EventTypes.ViewAccessibilityFocused);
            }
            catch (ObjectDisposedException e)
            {
                Logger.LogError(e, "Webview has been disposed and we cannot send the accessibility event");
            }
        }

        public void Dispose()
        {
            if (_renderer.Element is IAccessibleControl view)
            {
                // ReSharper disable once EventUnsubscriptionViaAnonymousDelegate
                view.AccessibilityFocusChangeRequested -= (sender, args) => SendViewAccessibilityEvent();
            }
        }
    }
}