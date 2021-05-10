using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class PkbPageContent
    {
        private readonly IWebInteractor _interactor;
        private readonly string _phrPath;

        internal PkbPageContent(IWebInteractor interactor, string phrPath)
        {
            _interactor = interactor;
            _phrPath = phrPath;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Pkb Internal Page");

        private WebText PhrPath => WebText.WithTagAndText(_interactor, "li", $"phrPath: {_phrPath}");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
            PhrPath.AssertVisible();
        }
    }
}