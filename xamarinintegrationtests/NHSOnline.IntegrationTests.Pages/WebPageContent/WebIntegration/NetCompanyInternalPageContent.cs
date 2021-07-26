using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class NetCompanyInternalPageContent
    {
        private readonly IWebInteractor _interactor;

        internal NetCompanyInternalPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "NetCompany Internal Page");

        private WebLink NhsLoginUpliftLink => WebLink.WithText(_interactor, "NHS Login Uplift");

        internal void AssertOnPage() => Title.AssertVisible();

        public NetCompanyInternalPageContent AssertPageContent()
        {
            NhsLoginUpliftLink.AssertVisible();
            return this;
        }

        public void Uplift() => NhsLoginUpliftLink.Click();
    }
}