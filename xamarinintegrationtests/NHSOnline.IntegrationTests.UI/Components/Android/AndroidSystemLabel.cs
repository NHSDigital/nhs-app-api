using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidSystemLabel
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _pattern;

        private AndroidSystemLabel(IAndroidInteractor interactor, string pattern)
        {
            _interactor = interactor;
            _pattern = pattern;
        }

        public static AndroidSystemLabel WhichMatches(IAndroidInteractor interactor, string pattern)
            => new(interactor, pattern);

        public void Click()
            => ActOnElement(e => e.Click());

        public bool IsPresent()
        {
            var isPresent = false;
            _interactor.ActOnDriver((_, findBy) =>
            {
                try
                {
                    var element = findBy(FindBy);
                    isPresent = element != null;
                }
                catch (NoSuchElementException)
                {
                    isPresent = false;
                }
            });
            return isPresent;
        }

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a label with matches {0} should be displayed", _pattern));

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy => MobileBy.AndroidUIAutomator($"new UiSelector().className(\"android.widget.TextView\").textMatches({_pattern.QuoteUiAutomatorLiteral()})");
    }
}