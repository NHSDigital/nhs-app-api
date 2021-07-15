namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public static class WebInteractorExtensions
    {
        public static string GetUserAgent(this IWebInteractor interactor)
        {
            return interactor.ExecuteJavascript("return window.navigator.userAgent;");
        }
    }
}