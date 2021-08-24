using System.Threading.Tasks;
using EventKit;
using EventKitUI;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.iOS.DependencyServices.Calendar;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;

namespace NHSOnline.App.iOS.Controllers
{
    public class CalendarEventController: IosViewController
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(CalendarEventController));

        public static async Task ShowEventController(AddEventToCalendarRequest request)
        {
            Logger.LogInformation("Showing event controller");

            await InvokeUIKitOnMainUIThread(async () =>
            {
                var eventController = new EKEventEditViewController
                {
                    EventStore = ApplicationEventStore.Current.EventStore
                };

                CreateEventControllerDelegate(eventController, request);

                await DisplayController(eventController).ResumeOnThreadPool();
            }).ResumeOnThreadPool();
        }

        public static async Task ShowBlankEventController()
        {
            Logger.LogInformation("Showing blank event controller");

            await InvokeUIKitOnMainUIThread(async () =>
            {
                var eventController = new EKEventEditViewController
                {
                    EventStore = ApplicationEventStore.Current.EventStore
                };

                CreateBlankEventControllerDelegate(eventController);

                await DisplayController(eventController).ResumeOnThreadPool();
            }).ResumeOnThreadPool();
        }

        private static void CreateEventControllerDelegate(
            EKEventEditViewController eventController,
            AddEventToCalendarRequest request)
        {
            var eventControllerDelegate = new CreateEventEditView(eventController);
            eventController.EditViewDelegate = eventControllerDelegate;

            eventController.Event = CreateCalendarEvent(request);
        }

        private static void CreateBlankEventControllerDelegate(
            EKEventEditViewController eventController)
        {
            var eventControllerDelegate = new CreateEventEditView(eventController);
            eventController.EditViewDelegate = eventControllerDelegate;

            eventController.Event = CreateBlankCalendarEvent();
        }

        private static EKEvent CreateCalendarEvent(AddEventToCalendarRequest request)
        {
            EKEvent calendarEvent = EKEvent.FromStore(ApplicationEventStore.Current.EventStore);
            calendarEvent.StartDate = NSDate.FromTimeIntervalSince1970(request.StartTimeEpochInSeconds.GetValueOrDefault());
            calendarEvent.EndDate = NSDate.FromTimeIntervalSince1970(request.EndTimeEpochInSeconds.GetValueOrDefault());
            calendarEvent.Title = request.Subject;
            calendarEvent.Notes = request.Body;
            calendarEvent.Location = request.Location;
            calendarEvent.Calendar = ApplicationEventStore.Current.EventStore.DefaultCalendarForNewEvents;
            return calendarEvent;
        }

        private static EKEvent CreateBlankCalendarEvent()
        {
            EKEvent calendarEvent = EKEvent.FromStore(ApplicationEventStore.Current.EventStore);
            calendarEvent.Calendar = ApplicationEventStore.Current.EventStore.DefaultCalendarForNewEvents;
            return calendarEvent;
        }
    }
}