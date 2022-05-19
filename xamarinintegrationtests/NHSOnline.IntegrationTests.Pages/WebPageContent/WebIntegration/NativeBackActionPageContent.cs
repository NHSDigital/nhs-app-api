using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class NativeBackActionPageContent
    {
        private readonly IWebInteractor _interactor;

        internal NativeBackActionPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Web Integration Functionality - Native Back Action");

        private WebInputText NativeBackActionText => WebInputText.WithLabel(_interactor, "Enter function");

        private WebButton SimulateNativeButton => WebButton.WithText(_interactor, "Simulate back");

        private WebButton SetBackActionButton => WebButton.WithText(_interactor, "Set Back Action");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            SimulateNativeButton,
        };



        internal void AssertOnPage() => TitleText.AssertVisible();

        public void EnterNativeBackAction(String function) => NativeBackActionText.EnterText(function.ToString());

        public void ClickSimulateBackButton() => SimulateNativeButton.Click();

        public void ClickSetBackActionButton() => SetBackActionButton.Click();

    }
}