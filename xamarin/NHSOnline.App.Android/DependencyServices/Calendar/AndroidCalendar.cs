using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Dialogs;
using NHSOnline.App.Droid.DependencyServices.AlertDialog;
using NHSOnline.App.Droid.DependencyServices.Calendar;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidCalendar))]
namespace NHSOnline.App.Droid.DependencyServices.Calendar
{
    public class AndroidCalendar: ICalendar
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidCalendar));

        public async Task AddToCalendar(AddEventToCalendarRequest request)
        {
            Logger.LogInformation("Starting attempt to add to android calendar");

            try
            {
                CalendarIntent.StartCalendarIntentFromRequest(request);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to add event to calendar from request");
                await CreateAndShowCalendarFailAlert().ResumeOnThreadPool();
            }
        }

        public async Task ShowCalendarPermissionDeniedAlert()
        {
            Logger.LogInformation("Showing alert for failure to create calendar event");
            await CreateAndShowCalendarFailAlert().ResumeOnThreadPool();
        }

        public async Task ShowCalendarAlertWhenValidationFails()
        {
            Logger.LogInformation("Showing alert for invalid calendar event request data");
            await CreateAndShowCalendarFailAlert().ResumeOnThreadPool();
        }

        public Task<bool> RequestPermission()
        {
            Logger.LogInformation("Using intents in android means we dont need to request permission, returning true");
            return Task.FromResult(true);
        }

        private static async Task CreateAndShowCalendarFailAlert()
        {
            Logger.LogInformation("Failed to add to calendar, showing alert dialog");

            await AndroidDialogPresenter.CreateAndShowAlertDialog(
                new AddToCalendarValidationFailed(StartCalendarFromBlankCalendarIntent)).ResumeOnThreadPool();
        }

        private static Task StartCalendarFromBlankCalendarIntent()
        {
            CalendarIntent.StartCalendarFromBlankCalendarIntent();
            return Task.CompletedTask;
        }
    }
}