using System.Threading.Tasks;
using EventKit;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.Controllers;
using NHSOnline.App.iOS.DependencyServices.Calendar;
using NHSOnline.App.Threading;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosCalendar))]
namespace NHSOnline.App.iOS.DependencyServices.Calendar
{
    public sealed class IosCalendar : ICalendar
    {
        private const string AlertHeader = "Cannot save event";
        private const string AlertBody = "You can try adding the event to your calendar yourself.";
        private const string AlertButtonText = "OK";

        public async Task<bool> RequestPermission()
        {
            var completionSource = new TaskCompletionSource<bool>();

            ApplicationEventStore.Current.EventStore.RequestAccess(EKEntityType.Event, (granted, _) =>
                completionSource.SetResult(granted));

            return await completionSource.Task.ResumeOnThreadPool();
        }

        public void AddToCalendar(AddEventToCalendarRequest request)
        {
            CalendarEventController.ShowEventController(request);
        }

        public void ShowPermissionDeniedAlert()
        {
            AlertPopup.ShowAlertPopup(AlertHeader, AlertBody, AlertButtonText);
        }
    }
}