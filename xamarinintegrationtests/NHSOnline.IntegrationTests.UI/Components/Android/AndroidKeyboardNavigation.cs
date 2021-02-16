using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidKeyboardNavigation
    {
        private readonly IAndroidInteractor _interactor;
        private readonly ImmutableArray<IFocusable> _expectedFocusableElements;

        private IFocusable? _currentFocusable;

        private AndroidKeyboardNavigation(
            IAndroidInteractor interactor,
            IEnumerable<IFocusable> expectedFocusableElements)
        {
            _interactor = interactor;
            _expectedFocusableElements = expectedFocusableElements.ToImmutableArray();
        }

        public static AndroidKeyboardNavigation WithExpectedFocusableElements(
            IAndroidInteractor interactor,
            params IFocusable[] focusableElements)
            => new(interactor, focusableElements);
        
        public void TabTo(IFocusable focusable)
        {
            _expectedFocusableElements
                .Should()
                .Contain(f => f.IsTheSameAs(focusable), "element to tab to must be in the list of expected focusable elements for the page");

            while (focusable.IsNotTheSameAs(_currentFocusable))
            {
                _interactor.PressTabKey();
                _currentFocusable = FollowingFocusable(_currentFocusable);
            }
        }

        private IFocusable FollowingFocusable(IFocusable? currentFocusable)
        {
            if (currentFocusable == null)
            {
                return _expectedFocusableElements[0];
            }

            return FocusableElementsInOrder()
                .SkipWhile(f => f.IsNotTheSameAs(currentFocusable))
                .Skip(1)
                .First();

            IEnumerable<IFocusable> FocusableElementsInOrder()
                => _expectedFocusableElements.Concat(_expectedFocusableElements);
        }

        public void AssertFocusOrder()
        {
            var expectedDescriptions = _expectedFocusableElements.Select(fe => fe.ElementDescription);

            var actualDescriptions = GetDescriptionsOfElementsInTabOrder();

            actualDescriptions.Should().BeEquivalentTo(expectedDescriptions, opts => opts.WithStrictOrdering());
        }

        private List<string> GetDescriptionsOfElementsInTabOrder()
        {
            var descriptions = new List<string>();

            string? firstElementDescription = null;
            var returnedToFirstElement = false;

            while (! returnedToFirstElement)
            {
                _interactor.PressTabKey();
                _interactor.ActOnElement(
                    MobileBy.AndroidUIAutomator("new UiSelector().focused(true)"),
                    element =>
                    {
                        var description = GetDescription(element);
                        
                        if (string.Equals(firstElementDescription, description, StringComparison.Ordinal))
                        {
                            returnedToFirstElement = true;
                        }
                        else
                        {
                            descriptions.Add(description);
                            firstElementDescription ??= description;
                        }
                    });
            }

            return descriptions;
        }

        private string GetDescription(AndroidElement element)
            => new FocusableDescriptionBuilder {Tag = element.GetAttribute("className"), Text = element.Text}.Description;
    }
}
