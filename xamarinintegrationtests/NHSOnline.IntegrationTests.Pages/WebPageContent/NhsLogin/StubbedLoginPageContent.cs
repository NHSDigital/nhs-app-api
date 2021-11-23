using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.DeviceProperties;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsLogin
{
    public sealed class StubbedLoginPageContent
    {
        private readonly IWebInteractor _interactor;

        internal StubbedLoginPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebLink BackLink => WebLink.WithText(_interactor, "Back");

        private WebLink InternalPageLink => WebLink.WithText(_interactor, "Internal Page");

        private WebLink InternalPageNewWindowLink => WebLink.WithText(_interactor, "Internal Page (New Window)");

        private WebLink CovidLink => WebLink.WithText(_interactor, "Covid");

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "NHS Login");

        private WebInputText PatientIdWebInputText => WebInputText.WithLabel(_interactor, "Patient ID");

        private WebDefinitionTerm VectorsOfTrustDefinitionTerm => WebDefinitionTerm.WithTerm(_interactor, "vtr");

        private WebInputSubmit LoginButton => WebInputSubmit.WithText(_interactor, "Login");

        private WebCheckbox LoginTermsAndConditionsCheckbox => WebCheckbox.WithLabel(_interactor, "Show Login Terms and Conditions");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public StubbedLoginPageContent AssertVectorOfTrust()
        {
            VectorsOfTrustDefinitionTerm.AssertValue("[\"P5.Cp.Cd\", \"P5.Cp.Ck\", \"P5.Cm\", \"P9.Cp.Cd\", \"P9.Cp.Ck\", \"P9.Cm\"]");
            return this;
        }

        public void Login(Patient patient)
        {
            PatientIdWebInputText.EnterText(patient.Id);
            LoginButtonClick();
        }

        public void Back() => BackLink.Click();

        public void InternalPage() => InternalPageLink.Click();

        public void InternalPageNewWindow() => InternalPageNewWindowLink.Click();

        public void Covid() => CovidLink.Click();

        public void LoginButtonClick() => LoginButton.Click();

        public void LoginWithLoginTermsAndConditions(Patient patient)
        {
            LoginTermsAndConditionsCheckbox.Click();
            Login(patient);
        }

        public void AssertUserAgent(Platform platform) =>
            Assert.IsTrue(_interactor.GetUserAgent().Contains(platform.UserAgentDeviceTypePrefix(), StringComparison.InvariantCulture));
    }
}