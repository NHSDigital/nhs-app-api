namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public enum WebViewContext
    {
        // Contexts that will not be returned to within the test
        // but the web view may persist and should not be reused.
        OneOff,
        NhsLogin,
        NhsApp
    }
}