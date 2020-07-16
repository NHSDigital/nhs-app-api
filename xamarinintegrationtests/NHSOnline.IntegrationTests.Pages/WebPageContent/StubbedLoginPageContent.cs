using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public sealed class StubbedLoginPageContent
    {
        private readonly IWebInteractor _interactor;

        internal StubbedLoginPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText TitleText => new WebText(_interactor, "h1", "NHS Login");

        private WebInputText PatientIdWebInputText => new WebInputText(_interactor, "Patient ID");

        private WebInputSubmit LoginButton => new WebInputSubmit(_interactor, "Login");

        internal void AssertOnPage()
        {
            TitleText.AssertVisible();
        }

        public void Login(Patient patient)
        {
            PatientIdWebInputText.EnterText(patient.Id);
            LoginButton.Click();
        }
    }
}