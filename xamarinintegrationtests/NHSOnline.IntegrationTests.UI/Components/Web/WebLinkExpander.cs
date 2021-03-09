using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public class WebLinkExpander
    {
        private readonly IWebInteractor _interactor;
        private readonly string _containerLink;
        private readonly List<IActOnElementContext> _containerElements = new List<IActOnElementContext>();

        public TComponent Contains<TComponent>(Func<IWebInteractor, TComponent> createComponent) where TComponent: IActOnElementContext
        {
            var containedInteractor = _interactor.CreateContainedInteractor(FindByContainer);
            var component = createComponent(containedInteractor);
            _containerElements.Add(component);
            return component;
        }

        public void AssertCollapsed()
        {
            foreach (var component in _containerElements)
            {
                component.ActOnElementContext(c => c.Element.Size.Height.Should().Be(0));
            }
        }

        public void AssertExpanded()
        {
            foreach (var component in _containerElements)
            {
                component.ActOnElementContext(c => c.Element.Size.Height.Should().BeGreaterThan(0));
            }
        }

        private WebLinkExpander(IWebInteractor interactor, string containerLink)
        {
            _interactor = interactor;
            _containerLink = containerLink;
        }

        public static WebLinkExpander WithText(IWebInteractor interactor, string continerLink)
            => new WebLinkExpander(interactor, continerLink);

        public void AssertVisible() => ActOnElement(e => e.Displayed.Should().BeTrue("an expander with text '{1}' should be displayed", _containerLink));

        public void Toggle() => ActOnElement(e => e.Click());

        private void ActOnElement(Action<IWebElement> action) => _interactor.ActOnElement(FindBy, action);
        
        private By FindByContainer => By.XPath($"//span[normalize-space()={_containerLink.QuoteXPathLiteral()}]/../..");

        private By FindBy => By.XPath($"//span[normalize-space()={_containerLink.QuoteXPathLiteral()}]");
    }
}