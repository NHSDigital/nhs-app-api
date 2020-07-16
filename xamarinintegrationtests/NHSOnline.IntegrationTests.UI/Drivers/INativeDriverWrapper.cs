namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface INativeDriverWrapper : IDriverWrapper
    {
        IWebInteractor Web(WebViewContext webViewContext);
    }
}