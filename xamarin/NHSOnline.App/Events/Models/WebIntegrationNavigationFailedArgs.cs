using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.Events.Models
{
    public sealed class WebIntegrationNavigationFailedArgs
    {
        public WebIntegrationRequest FailedRequest { get; }
        public bool OnInitialNavigation { get; }

        public WebIntegrationNavigationFailedArgs(WebIntegrationRequest failedRequest, bool onInitialNavigation)
        {
            FailedRequest = failedRequest;
            OnInitialNavigation = onInitialNavigation;
        }
    }
}