using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class StubbedLoginPaycassoPageContent
    {
        private readonly IWebInteractor _interactor;

        internal StubbedLoginPaycassoPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Paycasso SDK");

        private WebInputText TokenInput => WebInputText.WithLabel(_interactor, "Insert token");
        private WebInputText PaycassoUrlInput => WebInputText.WithLabel(_interactor, "Insert paycasso url (optional)");

        private WebInputSubmit StartButton => WebInputSubmit.WithText(_interactor, "Start");

        internal void AssertOnPage()
        {
            TitleText.AssertVisible();
        }

        public StubbedLoginPaycassoPageContent UseStubs()
        {
            TokenInput.EnterText("389c276f-bf32-4cb2-a101-7efd6d9043aa");
            PaycassoUrlInput.EnterText("http://paycasso.stubs.local.bitraft.io:8080/");
            return this;
        }

        public void Start()
        {
            StartButton.Click();
        }
    }
}