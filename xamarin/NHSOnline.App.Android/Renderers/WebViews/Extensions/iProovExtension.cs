using System;
using System.Diagnostics.CodeAnalysis;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using iProov.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "iProov is the name of the product")]
    internal sealed class IProovExtension: WebViewRendererExtension, IDisposable
    {
        private readonly WebViewRenderer _renderer;

        public IProovExtension(WebViewRenderer renderer) => _renderer = renderer;

        internal override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement == null)
            {
                // There are cryptographic APIs available via the NativeBridge that we do not use.
                // The isCryptographyEnabled parameter allows the enabling, and in our case disabling, of these APIs.
                IProov.NativeBridge.Install(_renderer.Control, false);
            }
        }

        public void Dispose()
        {
            IProov.NativeBridge.Uninstall(_renderer.Control);
        }
    }
}