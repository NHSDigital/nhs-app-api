namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public sealed class AddEventToCalendarRequest
    {
        public AddEventToCalendarRequest(
            string subject,
            string body,
            string location,
            int? startTimeEpochInSeconds,
            int? endTimeEpochInSeconds)
        {
            Subject = subject;
            Body = body;
            Location = location;
            StartTimeEpochInSeconds = startTimeEpochInSeconds;
            EndTimeEpochInSeconds = endTimeEpochInSeconds;
        }

        public string Subject { get; set; }
        public string Body { get; set; }
        public string Location { get; set; }
        public int? StartTimeEpochInSeconds { get; set; }
        public int? EndTimeEpochInSeconds { get; set; }
    }
}