using EventKit;
using EventKitUI;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.iOS.DependencyServices.Calendar;
using NHSOnline.App.Logging;

namespace NHSOnline.App.iOS.Controllers
{
    public class CalendarEventController: IosViewController
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(CalendarEventController));

        public static void ShowEventController(AddEventToCalendarRequest request)
        {
            Logger.LogInformation("Showing event controller");

            InvokeUIKitOnMainUIThread(() =>
            {
                var eventController = new EKEventEditViewController
                {
                    EventStore = ApplicationEventStore.Current.EventStore
                };

                CreateEventControllerDelegate(eventController, request);

                DisplayController(eventController);
            });
        }

        private static void CreateEventControllerDelegate(
            EKEventEditViewController eventController,
            AddEventToCalendarRequest request)
        {
            var eventControllerDelegate = new CreateEventEditView(eventController);
            eventController.EditViewDelegate = eventControllerDelegate;

            eventController.Event = CreateCalendarEvent(request);
        }

        private static EKEvent CreateCalendarEvent(AddEventToCalendarRequest request)
        {
            EKEvent calendarEvent = EKEvent.FromStore(ApplicationEventStore.Current.EventStore);
            calendarEvent.StartDate = NSDate.FromTimeIntervalSince1970(request.StartTimeEpochInSeconds);
            calendarEvent.EndDate = NSDate.FromTimeIntervalSince1970(request.EndTimeEpochInSeconds);
            calendarEvent.Title = request.Subject;
            calendarEvent.Notes = request.Body;
            calendarEvent.Location = request.Location;
            calendarEvent.Calendar = ApplicationEventStore.Current.EventStore.DefaultCalendarForNewEvents;
            return calendarEvent;
        }
    }
}