using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidCalendarTimeLabel
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _prefixText;
        private readonly string _twelveHourTime;
        private readonly string _twentyFourHourTime;


        private AndroidCalendarTimeLabel(
            IAndroidInteractor interactor,
            string prefixText,
            string twelveHourTime,
            string twentyFourHourTime)
        {
            _interactor = interactor;
            _prefixText = prefixText;
            _twelveHourTime = twelveHourTime;
            _twentyFourHourTime = twentyFourHourTime;
        }

        public static AndroidCalendarTimeLabel WithContentDescription(
            IAndroidInteractor interactor,
            string prefixText,
            string twelveHourTime,
            string twentyFourHourTime)
            => new(interactor, prefixText, twelveHourTime, twentyFourHourTime);

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                FindBy,
                e => e.Displayed.Should().BeTrue($"a cell with the label {_prefixText} and either {_twelveHourTime} or {_twentyFourHourTime} should be displayed"));
        }

        public void Click()
        {
            _interactor.ActOnElementContext(FindBy, context=>context.Element.Click());
        }

        private By FindBy
            => By.XPath(
                $"//*[contains(normalize-space(@content-desc), {_prefixText.QuoteXPathLiteral()}) and " +
                $"(contains(normalize-space(@content-desc), {_twelveHourTime.QuoteXPathLiteral()}) or " +
                $"contains(normalize-space(@content-desc), {_twentyFourHourTime.QuoteXPathLiteral()}))]");
    }
}
