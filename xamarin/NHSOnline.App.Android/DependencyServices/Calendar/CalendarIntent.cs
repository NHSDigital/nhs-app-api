using System;
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

            calendarIntent.PutExtra(CalendarContract.ExtraEventBeginTime,
                DateTimeOffset.FromUnixTimeSeconds(request.StartTimeEpochInSeconds.GetValueOrDefault()).ToUnixTimeMilliseconds());
            calendarIntent.PutExtra(CalendarContract.ExtraEventEndTime,
                DateTimeOffset.FromUnixTimeSeconds(request.EndTimeEpochInSeconds.GetValueOrDefault()).ToUnixTimeMilliseconds());

            calendarIntent.AddFlags(ActivityFlags.NewTask);

            Android.App.Application.Context.StartActivity(calendarIntent);
        }

        public static void StartCalendarFromBlankCalendarIntent()
        {
            using var calendarIntent = new Intent(Intent.ActionInsert);
            calendarIntent.SetData(CalendarContract.Events.ContentUri);
            calendarIntent.AddFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(calendarIntent);
        }
    }
}