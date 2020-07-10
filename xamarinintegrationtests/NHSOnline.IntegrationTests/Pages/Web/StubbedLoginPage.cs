using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Web
{
    internal sealed class StubbedLoginPage
    {
        private readonly IWebInteractor _interactor;

        private StubbedLoginPage(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => new WebText(_interactor, "h1", "NHS Login");

        private WebInputText PatientIdWebInputText => new WebInputText(_interactor, "Patient ID");

        private WebInputSubmit LoginButton => new WebInputSubmit(_interactor, "Login");

        internal static StubbedLoginPage AssertOnPage(IWebInteractor interactor)
        {
            var page = new StubbedLoginPage(interactor);
            page.TitleText.AssertVisible();
            return page;
        }

        internal StubbedLoginPage Login(Patient patient)
        {
            PatientIdWebInputText.EnterText(patient.Id);
            LoginButton.Click();
            return this;
        }
    }
}