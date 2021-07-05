using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.DeviceProperties;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class UpliftShutterPageContent
    {
        private readonly IWebInteractor _interactor;

        internal UpliftShutterPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebPanel UpliftPanel => WebPanel.WithTitle(_interactor, "Prove your identity to get full access");

        private WebButton Continue => UpliftPanel.ContainingButtonWithText("Continue");

        public void ProveYourIdentityContinue() => Continue.Click();
    }
}