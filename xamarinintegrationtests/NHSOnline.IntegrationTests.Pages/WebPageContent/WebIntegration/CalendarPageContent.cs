using System.Globalization;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class CalendarPageContent
    {
        private readonly IWebInteractor _interactor;

        internal CalendarPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Silver Integration Test Provider Calendar Page");

        private WebInputText SubjectInputText => WebInputText.WithLabel(_interactor, "Title");

        private WebInputText BodyInputText => WebInputText.WithLabel(_interactor, "Notes");
        private WebInputText ToFailText => WebInputText.WithLabel(_interactor, "Fail");

        private WebInputText LocationInputText => WebInputText.WithLabel(_interactor, "Location");
        private WebInputText StartTimeEpochInputText => WebInputText.WithLabel(_interactor, "Epoch Start Time");

        private WebInputText EndTimeEpochInputText => WebInputText.WithLabel(_interactor, "Epoch End Time");

        private WebButton AddToCalendarButton => WebButton.WithText(_interactor, "Add To Calendar");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AddToCalendar(int startTime, int endTime)
        {
            SubjectInputText.EnterText("Test Subject");
            BodyInputText.EnterText("Test Body");
            LocationInputText.EnterText("Test Location");
            StartTimeEpochInputText.EnterText(startTime.ToString(CultureInfo.InvariantCulture));
            EndTimeEpochInputText.EnterText(endTime.ToString(CultureInfo.InvariantCulture));
        }

        public void AddCalendarEvent() => AddToCalendarButton.Click();
    }
}