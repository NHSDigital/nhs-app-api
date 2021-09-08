using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth.Ndop
{
    public class NdopPageContent
    {
        private readonly IWebInteractor _interactor;

        internal NdopPageContent(IWebInteractor interactor)=>_interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Overview");
        private WebLink MakeYourChoiceLink => WebLink.WithText(_interactor, "Make your choice");
        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage() => TitleText.AssertVisible();

        internal void AssertElements()
        {
            BackBreadcrumb.AssertVisible();
            MakeYourChoiceLink.AssertVisible();
        }

        public void ClickMakeYourChoiceLink() => MakeYourChoiceLink.Click();

        public void ClickBackBreadcrumb() => BackBreadcrumb.Click();
    }
}