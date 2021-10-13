using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class AToZPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AToZPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Health A to Z");
        private WebLink BackLink => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage() => Title.AssertVisible();

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[] { BackLink };
    }
}