using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class OneOneOnePageContent
    {
        private readonly IWebInteractor _interactor;

        internal OneOneOnePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "111");

        private WebLink BackLink => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[] { BackLink };
    }
}