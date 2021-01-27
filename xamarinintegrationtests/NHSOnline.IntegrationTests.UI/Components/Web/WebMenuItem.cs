using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebMenuItem
    {
        private readonly IWebInteractor _interactor;
        private readonly string _title;

        private WebMenuItem(IWebInteractor interactor, string title)
        {
            _interactor = interactor;
            _title = title;
        }

        public static WebMenuItem WithTitle(IWebInteractor interactor, string title)
            => new WebMenuItem(interactor, title);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("A menu item with title {0} should be displayed", _title));

        public void Click()
            => ActOnElement(e => e.Click());

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//a[span/div/h2[normalize-space(text())={_title.QuoteXPathLiteral()}]]");
    }
}