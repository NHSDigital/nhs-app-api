using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidCalendarDateLabel
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _prefixText;
        private readonly string _month;
        private readonly string _date;
        private readonly string _year;


        private AndroidCalendarDateLabel(
            IAndroidInteractor interactor,
            string prefixText,
            string month,
            string date,
            string year)
        {
            _interactor = interactor;
            _prefixText = prefixText;
            _month = month;
            _date = date;
            _year = year;
        }

        public static AndroidCalendarDateLabel WithContentDescription(
            IAndroidInteractor interactor,
            string prefixText,
            string month,
            string date,
            string year)
            => new(interactor, prefixText,month,date,year);

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                FindBy,
                e => e.Displayed.Should().BeTrue("a date time label should be displayed"));
        }

        public void Click()
        {
            _interactor.ActOnElementContext(FindBy, context=>context.Element.Click());
        }

        private By FindBy
            => By.XPath(
                $"//*[contains(normalize-space(@content-desc), {_prefixText.QuoteXPathLiteral()}) and " +
                $"contains(normalize-space(@content-desc), {_date.QuoteXPathLiteral()}) and " +
                $"contains(normalize-space(@content-desc), {_month.QuoteXPathLiteral()}) and " +
                $"contains(normalize-space(@content-desc), {_year.QuoteXPathLiteral()}) ]");
    }
}
