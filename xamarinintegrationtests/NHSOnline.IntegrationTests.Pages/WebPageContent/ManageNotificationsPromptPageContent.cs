using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public sealed class ManageNotificationsPromptPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ManageNotificationsPromptPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Manage notifications");

        private WebText FallBackTitleText => WebText.WithTagAndText(_interactor, "h1", "Sorry, we could not set your notifications choice");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        private WebToggle NotificationsToggle => WebToggle.WithLabel(
            _interactor,
            "Allow notificationsI accept the NHS App sending notifications on this device");

        internal void AssertOnPage()
        {
            using var timeout = ExtendedTimeout.FromSeconds(10);

            try
            {
                TitleText.AssertVisible();
            }
            catch (AssertFailedException)
            {
                FallBackTitleText.AssertVisible();
            }
        }

        public void Continue() => ContinueButton.Click();

        public ManageNotificationsPromptPageContent ToggleOnNotifications()
        {
            NotificationsToggle.ToggleOn();
            using var timeout = ExtendedTimeout.FromSeconds(10);
            NotificationsToggle.AssertToggledOn();
            return this;
        }
    }
}