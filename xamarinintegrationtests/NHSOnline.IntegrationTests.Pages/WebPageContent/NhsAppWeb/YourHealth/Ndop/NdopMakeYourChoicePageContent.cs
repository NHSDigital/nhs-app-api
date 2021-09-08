using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth.Ndop
{
    public class NdopMakeYourChoicePageContent
    {
        private readonly IWebInteractor _interactor;

        internal NdopMakeYourChoicePageContent(IWebInteractor interactor)=>_interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Make your choice");
        private WebButton StartNowButton => WebButton.WithText(_interactor, "Start now");
        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        internal NdopMakeYourChoicePageContent AssertOnPage()
        {
            TitleText.AssertVisible();
            return this;
        }

        public NdopMakeYourChoicePageContent AssertElements()
        {
            BackBreadcrumb.AssertVisible();
            StartNowButton.AssertVisible();
            return this;
        }

        public void ClickStartButton() => StartNowButton.Click();
    }
}