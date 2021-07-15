using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices.Calendar;
using NHSOnline.App.Droid.Dialogs;
using NHSOnline.App.Logging;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidCalendar))]
namespace NHSOnline.App.Droid.DependencyServices.Calendar
{
    public class AndroidCalendar: ICalendar
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidCalendar));

        private const string Title = "Cannot save event";
        private const string Message = "You can try adding the event to your calendar yourself.";
        private const string AlertButtonNegativeText = "OK";
        private const string AlertButtonPositiveText = "Add event";

        public void AddToCalendar(AddEventToCalendarRequest request)
        {
            Logger.LogInformation("Starting attempt to add to android calendar");

            try
            {
                CalendarIntent.StartCalendarIntentFromRequest(request);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to add event to calendar from request");
                CreateAndShowCalendarFailAlert();
            }
        }

        public void ShowCalendarPermissionDeniedAlert()
        {
            Logger.LogInformation("Showing alert for failure to create calendar event");
            CreateAndShowCalendarFailAlert();
        }

        public void ShowCalendarAlertWhenValidationFails()
        {
            Logger.LogInformation("Showing alert for invalid calendar event request data");
            CreateAndShowCalendarFailAlert();
        }

        public Task<bool> RequestPermission()
        {
            Logger.LogInformation("Using intents in android means we dont need to request permission, returning true");
            return Task.FromResult(true);
        }

        private static void CreateAndShowCalendarFailAlert()
        {
            Logger.LogInformation("Failed to add to calendar, showing alert dialog");

            AndroidAlertDialog.ShowAlertDialog(
                Title,
                Message,
                negativeButtonText: AlertButtonNegativeText,
                positiveButtonText: AlertButtonPositiveText,
                positiveAction: CalendarIntent.StartCalendarFromBlankCalendarIntent);
        }
    }
}