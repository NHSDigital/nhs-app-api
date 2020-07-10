using NHSOnline.IntegrationTests.UI.Drivers.Native;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface INativeDriverWrapper : IDriverWrapper
    {
        INativeWebContext Web(WebViewContext webViewContext);
    }
}