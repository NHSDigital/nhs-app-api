using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebMenuItem: IFocusable
    {
        private readonly IWebInteractor _interactor;
        private readonly string _title;
        private readonly string _id;

        private WebMenuItem(IWebInteractor interactor, string title, string id)
        {
            _interactor = interactor;
            _title = title;
            _id = id;
        }

        public static WebMenuItem WithTitle(IWebInteractor interactor, string title)
            => new WebMenuItem(interactor, title, string.Empty);

        public static WebMenuItem WithTitle(IWebInteractor interactor, string title, string id)
            => new WebMenuItem(interactor, title, id);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("A menu item with title {0} should be displayed", _title));

        public void Click()
            => ActOnElement(e => e.Click());

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//a[div/h2[normalize-space(text())={_title.QuoteXPathLiteral()}]]");

        string IFocusable.ElementDescription
            => new FocusableDescriptionBuilder {Tag = "a", Text = _title, Id = _id}.Description;
    }
}