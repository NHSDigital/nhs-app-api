using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public sealed class ManageNotificationsPromptPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ManageNotificationsPromptPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Manage notifications");
        
        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public void Continue()
        {
            ContinueButton.Click();
        }
    }
}