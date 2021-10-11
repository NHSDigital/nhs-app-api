using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.Controls.WebViews
{
    public interface IPostRequestCapableWebView
    {
        WebIntegrationRequest? WebIntegrationRequest { get; }
    }
}