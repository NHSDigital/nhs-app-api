using System.Threading.Tasks;
using EventKit;
using Foundation;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.Controllers;
using NHSOnline.App.iOS.DependencyServices.AlertDialog;
using NHSOnline.App.iOS.DependencyServices.Calendar;
using NHSOnline.App.Threading;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosCalendar))]
namespace NHSOnline.App.iOS.DependencyServices.Calendar
{
    public sealed class IosCalendar : ICalendar
    {
        private const string AlertAddEventHeader = "Cannot save event";
        private const string AlertAddEventBody = "You can try adding the event to your calendar yourself.";
        private const string AlertGoToSettingsHeader = "Give NHS App calendar access";
        private const string AlertGoToSettingsBody =
            "NHS App does not have permission to add events to your calendar.\nGo to your device settings and allow access, then try again.";
        private const string OkButtonText = "OK";
        private const string AddEventButtonText = "Add event";
        private const string GoToSettingsButtonText = "Go to settings";

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

        public void ShowCalendarPermissionDeniedAlert()
        {
            Controllers.AlertDialogBox.ShowAlertPopup(
                ClearingActions.Dismissible,
                AlertGoToSettingsHeader,
                AlertGoToSettingsBody,
                OkButtonText,
                GoToSettingsButtonText,
                cancelAction: () =>
                {
                    using var settingsUrl = new NSUrl(UIApplication.OpenSettingsUrlString);
                    UIApplication.SharedApplication.OpenUrlAsync(settingsUrl, new UIApplicationOpenUrlOptions());
                });
        }

        public void ShowCalendarAlertWhenValidationFails()
        {
            Controllers.AlertDialogBox.ShowAlertPopup(
                ClearingActions.Dismissible,
                AlertAddEventHeader,
                AlertAddEventBody,
                OkButtonText,
                AddEventButtonText,
                cancelAction: () =>
                {
                    ApplicationEventStore.Current.EventStore.RequestAccess(EKEntityType.Event, (granted, _) =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (granted)
                            {
                                CalendarEventController.ShowBlankEventController();
                            }
                        });
                    });
                });
        }
    }
}