namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public sealed class AddEventToCalendarRequest
    {
        public AddEventToCalendarRequest(
            string subject,
            string body,
            string location,
            decimal? startTimeEpochInSeconds,
            decimal? endTimeEpochInSeconds)
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
        public decimal? StartTimeEpochInSeconds { get; set; }
        public decimal? EndTimeEpochInSeconds { get; set; }
    }
}