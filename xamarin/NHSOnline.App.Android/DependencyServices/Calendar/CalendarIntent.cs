using Android.Content;
using Android.Provider;
using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.Droid.DependencyServices.Calendar
{
    public static class CalendarIntent
    {
        public static void StartCalendarIntentFromRequest(AddEventToCalendarRequest request)
        {
            using var calendarIntent = new Intent(Intent.ActionInsert);

            calendarIntent.SetData(CalendarContract.Events.ContentUri);

            calendarIntent.PutExtra(CalendarContract.Events.InterfaceConsts.Title, request.Subject);
            calendarIntent.PutExtra(CalendarContract.Events.InterfaceConsts.Description, request.Body);
            calendarIntent.PutExtra(CalendarContract.Events.InterfaceConsts.EventLocation, request.Location);

            calendarIntent.PutExtra(CalendarContract.Events.InterfaceConsts.Availability,
                EventsAvailability.Busy.ToString());

            calendarIntent.PutExtra(CalendarContract.Events.InterfaceConsts.Dtstart,
                request.StartTimeEpochInSeconds * 1000);
            calendarIntent.PutExtra(CalendarContract.Events.InterfaceConsts.Dtend,
                request.EndTimeEpochInSeconds * 1000);

            calendarIntent.AddFlags(ActivityFlags.NewTask);

            Android.App.Application.Context.StartActivity(calendarIntent);
        }
    }
}