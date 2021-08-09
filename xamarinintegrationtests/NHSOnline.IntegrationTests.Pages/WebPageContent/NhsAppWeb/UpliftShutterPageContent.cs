using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class UpliftShutterPageContent
    {
        private const string Title = "Prove your identity to get full access";
        private readonly IWebInteractor _interactor;

        internal UpliftShutterPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h2", Title);

        private WebPanel UpliftPanel => WebPanel.WithTitle(_interactor, Title);

        private WebButton Continue => UpliftPanel.ContainingButtonWithText("Continue");

        internal void AssertOnPage() => TitleText.AssertVisible();

        internal void ProveYourIdentityContinue() => Continue.Click();
    }
}