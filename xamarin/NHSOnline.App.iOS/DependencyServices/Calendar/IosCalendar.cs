using System;
using System.Threading.Tasks;
using EventKit;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Dialogs;
using NHSOnline.App.iOS.Controllers;
using NHSOnline.App.iOS.DependencyServices.Calendar;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosCalendar))]
namespace NHSOnline.App.iOS.DependencyServices.Calendar
{
    public sealed class IosCalendar : ICalendar
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(IosCalendar));

        public async Task<bool> RequestPermission()
        {
            var completionSource = new TaskCompletionSource<bool>();

            ApplicationEventStore.Current.EventStore.RequestAccess(EKEntityType.Event, (granted, _) =>
                completionSource.SetResult(granted));

            return await completionSource.Task.ResumeOnThreadPool();
        }

        public async Task AddToCalendar(AddEventToCalendarRequest request)
        {
            try
            {
                await CalendarEventController.ShowEventController(request).ResumeOnThreadPool();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Add to iOS calendar failed");
            }
        }

        public async Task ShowCalendarPermissionDeniedAlert()
        {
            await AlertDialogBoxController.ShowAlertPopup(
                new AddToCalendarPermissionDenied(OpenSettings)).ResumeOnThreadPool();
        }

        public async Task ShowCalendarAlertWhenValidationFails()
        {
            await AlertDialogBoxController.ShowAlertPopup(
                new AddToCalendarValidationFailed(AddToBlankCalendar)).ResumeOnThreadPool();
        }

        private async Task OpenSettings()
        {
            using var settingsUrl = new NSUrl(UIApplication.OpenSettingsUrlString);
            await UIApplication.SharedApplication.OpenUrlAsync(settingsUrl, new UIApplicationOpenUrlOptions()).ResumeOnThreadPool();
        }

        private async Task AddToBlankCalendar()
        {
            var result = await ApplicationEventStore.Current.EventStore.RequestAccessAsync(EKEntityType.Event).ResumeOnThreadPool();
            if (result.Item1)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await CalendarEventController.ShowBlankEventController().ResumeOnThreadPool();
                }).ResumeOnThreadPool();
            }
        }
    }
}