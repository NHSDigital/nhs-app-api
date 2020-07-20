using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class SettingsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal SettingsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => new WebText(_interactor, "h1", "Settings");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public SettingsPageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }
    }
}