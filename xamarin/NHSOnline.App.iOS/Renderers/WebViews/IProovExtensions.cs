#if IPHONE
using iProov.iOS;
#endif
using System.Diagnostics.CodeAnalysis;
using Xamarin.Forms.Platform.iOS;

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "iProov is the name of the product")]
    internal static class IProovExtensions
    {
        internal static void InstallIProov(this WkWebViewRenderer renderer)
        {
#if IPHONE
            renderer.InstallIProovNativeBridge();
#endif
        }
    }
}