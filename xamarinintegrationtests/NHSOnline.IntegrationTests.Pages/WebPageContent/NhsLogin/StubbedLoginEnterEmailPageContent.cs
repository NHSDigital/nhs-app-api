using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.DeviceProperties;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsLogin
{
    public sealed class StubbedLoginEnterEmailPageContent
    {
        private readonly IWebInteractor _interactor;

        internal StubbedLoginEnterEmailPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "NHS login - Enter Email");

        internal void AssertOnPage() => TitleText.AssertVisible();
    }
}