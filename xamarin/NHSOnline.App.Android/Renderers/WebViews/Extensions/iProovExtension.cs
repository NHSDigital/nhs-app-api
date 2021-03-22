using System;
using System.Diagnostics.CodeAnalysis;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using iProov.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "iProov is the name of the product")]
    internal sealed class IProovExtension: IWebViewRendererExtension, IDisposable
    {
        private readonly WebViewRenderer _renderer;

        public IProovExtension(WebViewRenderer renderer) => _renderer = renderer;

        public void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement == null)
            {
                IProov.NativeBridge.Install(_renderer.Control);
            }
        }

        public void Dispose()
        {
            IProov.NativeBridge.Uninstall(_renderer.Control);
        }
    }
}